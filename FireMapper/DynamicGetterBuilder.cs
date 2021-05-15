using System;
using System.Reflection;
using System.Reflection.Emit;
namespace FireMapper
{
    public class DynamicGetterBuilder
    {
        private readonly AssemblyBuilder ab;
        private readonly ModuleBuilder mb;
        private readonly Type domain;
        private readonly AssemblyName aName;

        public DynamicGetterBuilder(Type domain)
        {
            this.domain = domain;
            aName = new AssemblyName(domain.Name + "Getters");
            ab = AssemblyBuilder.DefineDynamicAssembly(
                     aName,
                     AssemblyBuilderAccess.RunAndCollect);
            mb = ab.DefineDynamicModule(aName.Name + ".dll");
        }

        // public void SaveModule()
        // {
        //     ab.Save(aName.Name + ".dll");
        // }

        public Type GenerateSimpleGetter(PropertyInfo p)
        {
            // 1. Define the type
            TypeBuilder getterType = mb.DefineType(
                domain.Name + p.Name + "Getter", TypeAttributes.Public, typeof(AbstractGetter));

            // 2. Define the paraneterless constructor
            BuildSimpleConstructor(getterType, p);

            // 3. Define the method GetValue
            BuildGetValue(getterType, p);
            //BuildIsDefined(getterType, p);
            //BuildPropertyType(getterType, p);
            //BuildFillDictionary(getterType, p);
            // Finish type
            return getterType.CreateType();
        }

        private void BuildFillDictionary(TypeBuilder getterType, PropertyInfo p)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "FillDictionary",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[] { typeof(object) });
            //             IL_0000:  ldarg.2
            //   IL_0001:  castclass  Student
            //   IL_0006:  stloc.0
            //   IL_0007:  ldarg.1
            //   IL_0008:  ldstr      "name"
            //   IL_000d:  ldloc.0
            //   IL_000e:  callvirt   instance string Student::get_name()
            //   IL_0013:  callvirt   instance void class [System.Collections]System.Collections.Generic.Dictionary`2<string,object>::Add(!0,
            //                                                                                                                            !1)
            //   IL_0018:  ldarg.1
            //   IL_0019:  ret

            MethodInfo getFieldMethod = p.GetGetMethod();
            ILGenerator il = getValBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_2);             // ldarg.1
            il.Emit(OpCodes.Castclass, domain);   // castclass  Student
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldstr, p.Name);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Callvirt, getFieldMethod);
            // get_field --> number, name , classroom
            if (getFieldMethod.ReturnType.IsPrimitive)
            {
                il.Emit(OpCodes.Box, getFieldMethod.ReturnType);
            }
            il.Emit(OpCodes.Callvirt, getFieldMethod);
            il.Emit(OpCodes.Ret);                 // ret
        }

        private void BuildPropertyType(TypeBuilder getterType, PropertyInfo p)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "PropertyType",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[] { typeof(object) });

            ILGenerator il = getValBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4_1);             // ldarg.1
            il.Emit(OpCodes.Ret);              // ret
        }

        private void BuildIsDefined(TypeBuilder getterType, PropertyInfo p)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "IsDefined",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[] { typeof(object) });

            ILGenerator il = getValBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4_1);             // ldarg.1
            il.Emit(OpCodes.Ret);                 // ret
        }
        private void BuildGetValue(TypeBuilder getterType, PropertyInfo p)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[] { typeof(object) });

            ILGenerator il = getValBuilder.GetILGenerator();
            MethodInfo getFieldMethod = p.GetGetMethod();
            il.Emit(OpCodes.Ldarg_1);             // ldarg.1
            il.Emit(OpCodes.Castclass, domain);   // castclass  Student
            il.Emit(OpCodes.Callvirt, getFieldMethod);
            // get_field --> number, name , classroom
            if (getFieldMethod.ReturnType.IsPrimitive)
            {
                il.Emit(OpCodes.Box, getFieldMethod.ReturnType);
            }
            il.Emit(OpCodes.Ret);                 // ret

        }


        private void BuildSimpleConstructor(TypeBuilder getterType, PropertyInfo p)
        {
            ConstructorBuilder ctor = getterType.DefineConstructor(
                MethodAttributes.Public,
                 CallingConventions.Standard,
                 Type.EmptyTypes); // <=> new Type[0]

            ILGenerator il = ctor.GetILGenerator();
            // IL_0000: ldarg.0
            // IL_0001: ldstr      "name"
            // IL_0006: call instance void [FireMapper]AbstractGetter::.ctor(string)
            // IL_000b: ret
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, p.Name);
            il.Emit(
                OpCodes.Call,              // call AbstractGetter::.ctor(string)
                typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Ret);
        }
        public Type GenerateComplexGetter(PropertyInfo p,
                                            string ProjectId,
                                            string OtherCollection,
                                            string CredentialsPath,
                                            Type dataSourceType)
        {
            // 1. Define the type
            TypeBuilder getterType = mb.DefineType(
                domain.Name + p.Name + "Getter", TypeAttributes.Public, typeof(AbstractGetter));
            FieldBuilder dbField = getterType.DefineField(
                "db",
                 typeof(IDataMapper),
                 FieldAttributes.Private);
            // 2. Define the paraneterless constructor
            BuildComplexConstructor(getterType, p, ProjectId, OtherCollection, CredentialsPath, dataSourceType);

            // 3. Define the method GetValue
            BuildComplexGetValue(getterType, p, dbField);

            // Finish type
            return getterType.CreateType();
        }

        private void BuildComplexGetValue(TypeBuilder getterType, PropertyInfo p, FieldBuilder dbField)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                 "GetValue",
                 MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                 typeof(object),
                 new Type[] { typeof(object) });

            ILGenerator il = getValBuilder.GetILGenerator();
            MethodInfo GetByIdMethod = dbField.FieldType.GetMethod("GetById", new Type[] { typeof(object) });
            il.Emit(OpCodes.Ldarg_0);             // ldarg.0
            il.Emit(OpCodes.Ldfld, dbField);   // load field db
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, GetByIdMethod);
            // get_field --> number, name , classroom
            if (GetByIdMethod.ReturnType.IsPrimitive)
            {
                il.Emit(OpCodes.Box, GetByIdMethod.ReturnType);
            }
            il.Emit(OpCodes.Ret);                 // ret

        }

        private void BuildComplexConstructor(TypeBuilder getterType,
                                                PropertyInfo p,
                                                string ProjectId,
                                                string OtherCollection,
                                                string CredentialsPath,
                                                Type dataSourceType)
        {
            ConstructorBuilder ctor = getterType.DefineConstructor(
                MethodAttributes.Public,
                 CallingConventions.Standard,
                 Type.EmptyTypes);
            // <=> new Type[0]
            //             IL_0000:  ldarg.0
            //   IL_0001:  ldstr      "classroom"
            //   IL_0006:  ldtoken    App.ClassroomInfo
            //   IL_000b:  call       class [System.Runtime]System.Type [System.Runtime]System.Type::GetTypeFromHandle(valuetype [System.Runtime]System.RuntimeTypeHandle)
            //   IL_0010:  ldstr      "ave-trab1-g02"
            //   IL_0015:  ldstr      "Classrooms"
            //   IL_001a:  ldstr      "D:/2Ano/2 semestre/AVE/Trab1/FireMapper/App/Resour"
            //   + "ces/ave-trab1-g02-firebase-adminsdk-3f705-1ab19a5fb2.json"
            //   IL_001f:  ldtoken    [FireSource]FireSource.FireDataSource
            //   IL_0024:  call       class [System.Runtime]System.Type [System.Runtime]System.Type::GetTypeFromHandle(valuetype [System.Runtime]System.RuntimeTypeHandle)
            //   IL_0029:  newobj     instance void [FireMapper]FireMapper.DynamicFireMapper::.ctor(class [System.Runtime]System.Type,
            //                                                                                      string,
            //                                                                                      string,
            //                                                                                      string,
            //                                                                                      class [System.Runtime]System.Type)
            //   IL_002e:  call       instance void [FireMapper]AbstractGetter::.ctor(string,
            //                                                                        class [FireMapper]FireMapper.IDataMapper)
            //   IL_0033:  ret
            MethodInfo getTypeFromhandle = typeof(Type).GetMethod("GetTypeFromHandle");
            // Getting the type referenced by
            // the specified RuntimeTypeHandle,
            // using GetTypeFromHandle() Method

            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);      // ldarg.0
            il.Emit(OpCodes.Ldstr, p.Name);// ld property name
            il.Emit(OpCodes.Ldtoken, p.PropertyType);
            il.Emit(OpCodes.Call, getTypeFromhandle);
            il.Emit(OpCodes.Ldstr, ProjectId);
            il.Emit(OpCodes.Ldstr, OtherCollection);
            il.Emit(OpCodes.Ldstr, CredentialsPath);
            il.Emit(OpCodes.Ldtoken, dataSourceType);
            il.Emit(OpCodes.Call, getTypeFromhandle);
            il.Emit(OpCodes.Newobj, typeof(DynamicFireMapper).GetConstructor(new Type[] { typeof(Type),
                                                                                        typeof(string),
                                                                                        typeof(string),
                                                                                        typeof(string),
                                                                                        typeof(Type)}));
            il.Emit(OpCodes.Call, typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string), typeof(IDataMapper) }));
            il.Emit(OpCodes.Ret);
            // ILGenerator il = ctor.GetILGenerator();
            // il.Emit(OpCodes.Ldarg_0);      // ldarg.0
            // il.Emit(OpCodes.Ldstr, p.Name);// ld PropertyInfo
            // il.Emit(OpCodes.Call, typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string) }));
            // il.Emit(OpCodes.Ldarg_0);
            // il.Emit(OpCodes.Ldarg_1);
            // il.Emit(OpCodes.Stfld, dbField);
            // il.Emit(OpCodes.Ret);
        }
    }
}