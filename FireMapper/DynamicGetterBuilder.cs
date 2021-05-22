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
                     AssemblyBuilderAccess.RunAndSave);
            mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
        }

        public void SaveModule()
        {
            ab.Save(aName.Name + ".dll");
        }

        public Type GenerateSimpleGetter(PropertyInfo p)
        {
            // 1. Define the type
            TypeBuilder getterType = mb.DefineType(
                domain.Name + p.Name + "Getter", TypeAttributes.Public, typeof(AbstractGetter));

            BuildSimpleConstructor(getterType, p);
            BuildSimpleGetValue(getterType, p);
            BuildSimpleGetKeyValue(getterType);
            BuildSimpleGetDefaultValue(getterType, p);
            // Finish type
            return getterType.CreateType();
        }

        private void BuildSimpleGetDefaultValue(TypeBuilder getterType, PropertyInfo p)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetDefaultValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[0]);
            ILGenerator il = getValBuilder.GetILGenerator();
            if (p.PropertyType.IsPrimitive)
            {
                il.Emit(OpCodes.Ldc_I4_0);
            }
            else if (p.PropertyType.IsValueType)
            {
                LocalBuilder a = il.DeclareLocal(p.PropertyType);  // declare method of type p
                il.Emit(OpCodes.Ldloca, a);
                il.Emit(OpCodes.Initobj, p.PropertyType);
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }
            il.Emit(OpCodes.Ret);
        }

        private void BuildSimpleGetKeyValue(TypeBuilder getterType)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetKeyValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[] { typeof(object) });
            //   IL_0000:  ldarg.0
            //   IL_0001:  ldarg.1
            //   IL_0002:  callvirt   instance object [FireMapper]AbstractGetter::GetValue(object)
            //   IL_0007:  ret
            MethodInfo getValue = typeof(AbstractGetter).GetMethod("GetValue");
            ILGenerator il = getValBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, getValue);
            il.Emit(OpCodes.Ret);
        }

        public Type GenerateComplexGetter(PropertyInfo p)
        {
            // 1. Define the type
            TypeBuilder getterType = mb.DefineType(
                domain.Name + p.Name + "Getter", TypeAttributes.Public, typeof(AbstractGetter));

            // 2. Define the paraneterless constructor
            BuildComplexConstructor(getterType);
            // 3. Define the method GetValue
            BuildComplexGetValue(getterType, p);
            // 4. Define method GetKeyValue
            BuildComplexGetKeyValue(getterType, p);
            BuildComplexGetDefaultValue(getterType);

            // Finish type
            return getterType.CreateType(); ;
        }

        private void BuildComplexGetDefaultValue(TypeBuilder getterType)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetDefaultValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[0]);
            ILGenerator il = getValBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);
        }

        private void BuildComplexGetKeyValue(TypeBuilder getterType, PropertyInfo p)
        {
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetKeyValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[] { typeof(object) });
            //   IL_0000:  ldarg.0
            //   IL_0001:  ldfld      class [FireMapper]FireMapper.IDataMapper [FireMapper]AbstractGetter::db
            //   IL_0006:  callvirt   instance class [FireMapper]IGetter [FireMapper]FireMapper.IDataMapper::GetFireKey()
            //   IL_000b:  ldarg.1
            //   IL_000c:  castclass  App.Student
            //   IL_0011:  callvirt   instance class App.ClassroomInfo App.Student::get_classroom()
            //   IL_0016:  callvirt   instance object [FireMapper]IGetter::GetValue(object)
            //   IL_001b:  ret
            FieldInfo field = typeof(AbstractGetter).GetField("db");
            MethodInfo GetFireKeyMethod = field.FieldType.GetMethod("GetFireKey", new Type[0]);
            MethodInfo getFieldMethod = domain.GetMethod("get_" + p.Name);
            MethodInfo getValue = typeof(AbstractGetter).GetMethod("GetValue");
            ILGenerator il = getValBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Callvirt, GetFireKeyMethod);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, domain);
            il.Emit(OpCodes.Callvirt, getFieldMethod);
            il.Emit(OpCodes.Callvirt, getValue);
            il.Emit(OpCodes.Ret);
        }

        private void BuildSimpleGetValue(TypeBuilder getterType, PropertyInfo p)
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
                typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string)}));
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

        private void BuildComplexConstructor(TypeBuilder getterType)
        {
            ConstructorBuilder ctor = getterType.DefineConstructor(
                MethodAttributes.Public,
                 CallingConventions.Standard, new Type[] { typeof(string), typeof(IDataMapper) });
            //  IL_0000:  ldarg.0
            //   IL_0001:  ldarg.1
            //   IL_0002:  ldarg.2
            //   IL_0003:  call       instance void [FireMapper]AbstractGetter::.ctor(string,
            //                                                                        class [FireMapper]FireMapper.IDataMapper)
            //   IL_0008:  ret
            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string), typeof(IDataMapper) }));
            il.Emit(OpCodes.Ret);

            //------------------Another option------------------
            //MethodInfo getTypeFromhandle = typeof(Type).GetMethod("GetTypeFromHandle");

            // ILGenerator il = ctor.GetILGenerator();
            // il.Emit(OpCodes.Ldarg_0);      // ldarg.0
            // il.Emit(OpCodes.Ldstr, p.Name);// ld property name
            // il.Emit(OpCodes.Ldtoken, p.PropertyType);
            // il.Emit(OpCodes.Call, getTypeFromhandle);
            // il.Emit(OpCodes.Ldstr, ProjectId);
            // il.Emit(OpCodes.Ldstr, OtherCollection);
            // il.Emit(OpCodes.Ldstr, CredentialsPath);
            // il.Emit(OpCodes.Ldtoken, dataSourceType);
            // il.Emit(OpCodes.Call, getTypeFromhandle);
            // il.Emit(OpCodes.Newobj, typeof(DynamicFireMapper).GetConstructor(new Type[] { typeof(Type),
            //                                                                             typeof(string),
            //                                                                             typeof(string),
            //                                                                             typeof(string),
            //                                                                             typeof(Type)}));
            // il.Emit(OpCodes.Call, typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string), typeof(IDataMapper) }));
            // il.Emit(OpCodes.Ret);
        }

    }

}