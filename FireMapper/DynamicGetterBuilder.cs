using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
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

        public Type GenerateSimpleGetter(PropertyInfo p, bool isKey)
        {
            // 1. Define the type
            TypeBuilder getterType = mb.DefineType(
                domain.Name + p.Name + "Getter", TypeAttributes.Public, typeof(AbstractGetter));

            BuildSimpleConstructor(getterType, p, isKey);


            BuildGetValue(getterType, p);
            BuildFillDictionary(getterType, p);
            // Finish type
            return getterType.CreateType();
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

            // 2. Define the paraneterless constructor
            BuildComplexConstructor(getterType, p, ProjectId, OtherCollection, CredentialsPath, dataSourceType);
            // 3. Define the method GetValue
            BuildComplexGetValue(getterType, p);
            //BuildIsDefined(getterType, p);
            // Finish type
            return getterType.CreateType();
        }
        private void BuildFillDictionary(TypeBuilder getterType, PropertyInfo p)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "FillDictionary",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(Dictionary<string, object>),
                new Type[] { typeof(Dictionary<string, object>), typeof(object) });
            //   IL_0000:  ldarg.2
            //   IL_0001:  castclass  App.Student
            //   IL_0006:  stloc.0
            //   IL_0007:  ldarg.1
            //   IL_0008:  ldarg.0
            //   IL_0009:  ldfld      string [FireMapper]AbstractGetter::name
            //   IL_000e:  ldloc.0
            //   IL_000f:  callvirt   instance string App.Student::get_name()
            //   IL_0014:  callvirt   instance void class [System.Collections]System.Collections.Generic.Dictionary`2<string,object>::Add(!0,
            //                                                                                                                            !1)
            //   IL_0019:  ldarg.1
            //   IL_001a:  ret
            ILGenerator il = getValBuilder.GetILGenerator();
            FieldInfo field = typeof(AbstractGetter).GetField("name");
            MethodInfo getFieldMethod = p.GetGetMethod();
            MethodInfo DicAddMethod = typeof(Dictionary<string, object>).GetMethod("Add",new Type[] { typeof(string), typeof(object) } ) ;
            Label falseLabel = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_2);             // ldarg.1
            il.Emit(OpCodes.Castclass, domain);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Callvirt, getFieldMethod);
            il.Emit(OpCodes.Callvirt, DicAddMethod);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ret);
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
            Label falseLabel = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);             // ldarg.1
            il.Emit(OpCodes.Isinst, domain);
            il.Emit(OpCodes.Brfalse, falseLabel);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, domain);   // castclass  Student
            il.Emit(OpCodes.Callvirt, getFieldMethod);
            il.Emit(OpCodes.Box, getFieldMethod.ReturnType);
            // get_field --> number, name , classroom
            if (getFieldMethod.ReturnType.IsPrimitive)
            {
                il.Emit(OpCodes.Box, getFieldMethod.ReturnType);
            }
            il.Emit(OpCodes.Ret);                 // ret
            il.MarkLabel(falseLabel);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ret);
        }


        private void BuildSimpleConstructor(TypeBuilder getterType, PropertyInfo p, bool isKey)
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
            if (isKey)
                il.Emit(OpCodes.Ldc_I4_1);
            else
                il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(
                OpCodes.Call,              // call AbstractGetter::.ctor(string)
                typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string), typeof(bool) }));
            il.Emit(OpCodes.Ret);
        }


        private void BuildComplexGetValue(TypeBuilder getterType, PropertyInfo p)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                 "GetValue",
                 MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                 typeof(object),
                 new Type[] { typeof(object) });
            FieldInfo field = typeof(AbstractGetter).GetField("db");
            ILGenerator il = getValBuilder.GetILGenerator();
            MethodInfo GetByIdMethod = field.FieldType.GetMethod("GetById", new Type[] { typeof(object) });
            il.Emit(OpCodes.Ldarg_0);             // ldarg.0
            il.Emit(OpCodes.Ldfld, field);   // load field db
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, GetByIdMethod);
            // get_field --> number, name , classroom
            // if (GetByIdMethod.ReturnType.IsPrimitive)
            // {
            //     il.Emit(OpCodes.Box, GetByIdMethod.ReturnType);
            // }
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
        }

    }

}