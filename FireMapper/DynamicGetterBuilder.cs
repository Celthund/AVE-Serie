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
                il.Emit(OpCodes.Box, p.PropertyType);
            }
            else if (p.PropertyType.IsValueType)
            {
                LocalBuilder a = il.DeclareLocal(p.PropertyType);  // declare method of type p
                il.Emit(OpCodes.Ldloca, a);
                il.Emit(OpCodes.Initobj, p.PropertyType);
                il.Emit(OpCodes.Box, p.PropertyType);
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
            // 5. Define method GetDefaultValue
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
            FieldInfo field = typeof(AbstractGetter).GetField("db");
            MethodInfo GetFireKeyMethod = field.FieldType.GetMethod("GetFireKey", new Type[0]);
            MethodInfo getFieldMethod = domain.GetMethod("get_" + p.Name);
            MethodInfo getValue = typeof(IGetter).GetMethod("GetValue");
            ILGenerator il = getValBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Callvirt, GetFireKeyMethod);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, domain);
            il.Emit(OpCodes.Callvirt, getFieldMethod);
            il.Emit(OpCodes.Callvirt, getValue);
            if (p.PropertyType.IsPrimitive){
                il.Emit(OpCodes.Box, p.PropertyType );
            }
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
            il.Emit(OpCodes.Ldarg_1);             
            il.Emit(OpCodes.Isinst, domain);
            il.Emit(OpCodes.Brfalse, falseLabel);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, domain);   
            il.Emit(OpCodes.Callvirt, getFieldMethod);
            if (getFieldMethod.ReturnType.IsValueType)
            {
                il.Emit(OpCodes.Box, getFieldMethod.ReturnType);
            }
            il.Emit(OpCodes.Ret);                
            il.MarkLabel(falseLabel);
            il.Emit(OpCodes.Ldarg_1);
            if (getFieldMethod.ReturnType.IsValueType) {
                MethodInfo changeType = typeof(Convert).GetMethod("ChangeType",
                                        new Type[] { typeof(object), typeof(Type) });
                Label sameType = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Isinst, p.PropertyType);
                il.Emit(OpCodes.Brtrue, sameType);
                il.Emit(OpCodes.Ldtoken, p.PropertyType);
                il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle)}));
                il.Emit(OpCodes.Call, changeType);
                il.MarkLabel(sameType);
            }
            il.Emit(OpCodes.Ret);
        }

        private void BuildSimpleConstructor(TypeBuilder getterType, PropertyInfo p)
        {
            ConstructorBuilder ctor = getterType.DefineConstructor(
                MethodAttributes.Public,
                 CallingConventions.Standard,
                 Type.EmptyTypes); // <=> new Type[0]

            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, p.Name);
            il.Emit(
                OpCodes.Call,              
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
            il.Emit(OpCodes.Ldarg_0);            
            il.Emit(OpCodes.Ldfld, field);   
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, GetByIdMethod);
            il.Emit(OpCodes.Ret);                 

        }

        private void BuildComplexConstructor(TypeBuilder getterType)
        {
            ConstructorBuilder ctor = getterType.DefineConstructor(
                MethodAttributes.Public,
                 CallingConventions.Standard, new Type[] { typeof(string), typeof(IDataMapper) });
            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string), typeof(IDataMapper) }));
            il.Emit(OpCodes.Ret);
        }
    }
}