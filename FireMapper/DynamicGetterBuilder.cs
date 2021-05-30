using System;
using System.Reflection;
using System.Reflection.Emit;
namespace FireMapper
{
    public class DynamicGetterBuilder
    {
        //AssemblyBuilder
        private readonly AssemblyBuilder ab;
        //ModuleBuilder
        private readonly ModuleBuilder mb;
        //Domain Type
        private readonly Type domain;
        //Assmble Module name
        private readonly AssemblyName aName;

        /*
        Constructor
        */
        public DynamicGetterBuilder(Type domain)
        {
            this.domain = domain;
            aName = new AssemblyName(domain.Name + "Getters");
            ab = AssemblyBuilder.DefineDynamicAssembly(
                     aName,
                     AssemblyBuilderAccess.RunAndSave);
            mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
        }

        //Saves the dynamic created module
        public void SaveModule()
        {
            ab.Save(aName.Name + ".dll");
        }
        //Generates a simple Getter of the property
        public Type GenerateSimpleGetter(PropertyInfo p)
        {
            //Define the type
            TypeBuilder getterType = mb.DefineType(
                domain.Name + p.Name + "Getter", TypeAttributes.Public, typeof(AbstractGetter));
            //Builds the type Constructor 
            BuildSimpleConstructor(getterType, p);
            //Builds the type GetValue Method
            BuildSimpleGetValue(getterType, p);
            //Builds the type GetKeyValue Method
            BuildSimpleGetKeyValue(getterType);
            //Builds the type GetDefaultValue Method
            BuildSimpleGetDefaultValue(getterType, p);
            // Finish type
            return getterType.CreateType();
        }
        /*
        Generates a complex getter of the property
        */
        public Type GenerateComplexGetter(PropertyInfo p)
        {
            //Define the type
            TypeBuilder getterType = mb.DefineType(
                domain.Name + p.Name + "Getter", TypeAttributes.Public, typeof(AbstractGetter));
            //Builds the type Constructor 
            BuildComplexConstructor(getterType);
            //Builds the type GetValue Method
            BuildComplexGetValue(getterType, p);
            //Builds the type GetKeyValue Method
            BuildComplexGetKeyValue(getterType, p);
            //Builds the type GetDefaultValue Method
            BuildComplexGetDefaultValue(getterType);

            // Finish type
            return getterType.CreateType(); ;
        }
        /*
        Builds the type Constructor (Simple)
        */
        private void BuildSimpleConstructor(TypeBuilder getterType, PropertyInfo p)
        {
            //Constructor Builder
            ConstructorBuilder ctor = getterType.DefineConstructor(
                MethodAttributes.Public,
                 CallingConventions.Standard,
                 Type.EmptyTypes);

            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);                         //Loads this in the stack
            il.Emit(OpCodes.Ldstr, p.Name);                   //Loads string property name in the stak
            //Calls AbstractGetter GetConstructor
            il.Emit(
                OpCodes.Call,
                typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Ret);
        }
        /*
        Builds the type GetValue Method (Simple)
        */
        private void BuildSimpleGetValue(TypeBuilder getterType, PropertyInfo p)
        {
            //Method Builder
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[] { typeof(object) });

            ILGenerator il = getValBuilder.GetILGenerator();
            MethodInfo getFieldMethod = p.GetGetMethod();        //Defines get property value method
            Label falseLabel = il.DefineLabel();                 //Defines label
            il.Emit(OpCodes.Ldarg_1);                            //Loads first argument in the stack
            il.Emit(OpCodes.Isinst, domain);                     //Checks if is instance of domain type
            il.Emit(OpCodes.Brfalse, falseLabel);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, domain);                  //Cast value in top of stack to domain type
            il.Emit(OpCodes.Callvirt, getFieldMethod);           //Calls get property value method
            if (getFieldMethod.ReturnType.IsValueType)           //Checks if the retuned value is ValueType
            {
                il.Emit(OpCodes.Box, getFieldMethod.ReturnType); //Boxes the returned value
            }
            il.Emit(OpCodes.Ret);
            il.MarkLabel(falseLabel);                             //Not instance of domain type
            il.Emit(OpCodes.Ldarg_1);
            if (getFieldMethod.ReturnType.IsValueType)            //Checks if the returned value is ValueType 
            {    
                //Defines ChangeType method of Convert type
                MethodInfo changeType = typeof(Convert).GetMethod("ChangeType",
                                        new Type[] { typeof(object), typeof(Type) }); 
                Label sameType = il.DefineLabel();                //Defines label
                il.Emit(OpCodes.Ldarg_1);                         //Loads first argument in the stack
                il.Emit(OpCodes.Isinst, p.PropertyType);          //Checks if is instance of property type
                il.Emit(OpCodes.Brtrue, sameType);
                il.Emit(OpCodes.Ldtoken, p.PropertyType);         //Loads property type handle in the stack
                //Calls GetTypeFromHandle Method
                il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) }));
                il.Emit(OpCodes.Call, changeType);                //Calls change type method
                il.MarkLabel(sameType);                           //Same type
            }
            il.Emit(OpCodes.Ret);
        }

        /*
        Builds the type GetDefaultValue Method (Simple)
        */
        private void BuildSimpleGetDefaultValue(TypeBuilder getterType, PropertyInfo p)
        {
            //Method Builder
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetDefaultValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[0]);
            ILGenerator il = getValBuilder.GetILGenerator();                                                                   
            if (p.PropertyType.IsPrimitive)                        //Checks if property type is Primitive
            {
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Box, p.PropertyType);              //Boxes the value with the property type
            }
            else if (p.PropertyType.IsValueType)                   //Checks property type is ValueType
            {
                LocalBuilder a = il.DeclareLocal(p.PropertyType);  //Declare local variable of the property type
                il.Emit(OpCodes.Ldloca, a);                        //Loads local variable address in the stack
                il.Emit(OpCodes.Initobj, p.PropertyType);          //Initiates the object
                il.Emit(OpCodes.Box, p.PropertyType);              //Boxes the value with the property type
            }
            else
            {
                il.Emit(OpCodes.Ldnull);                           //Loads null in the stack 
            }
            il.Emit(OpCodes.Ret);
        }
        /*
        Builds the type GetKeyValue Method (Simple)
        */
        private void BuildSimpleGetKeyValue(TypeBuilder getterType)
        {
            //Method Builder
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetKeyValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[] { typeof(object) });
            //Defines the GetValue Method of type AbstractGetter
            MethodInfo getValue = typeof(AbstractGetter).GetMethod("GetValue");  
            ILGenerator il = getValBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);                             //Loads this in the stack
            il.Emit(OpCodes.Ldarg_1);                             //Loads first parameter in the stack
            il.Emit(OpCodes.Callvirt, getValue);                  //Calls getValue method and puts the return value on the stack
            il.Emit(OpCodes.Ret);
        }

        /*
        Builds the type Constructor (Complex)
        */
        private void BuildComplexConstructor(TypeBuilder getterType)
        {
            //Constructor Builder
            ConstructorBuilder ctor = getterType.DefineConstructor(
                MethodAttributes.Public,
                 CallingConventions.Standard, new Type[] { typeof(string), typeof(IDataMapper) });
            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);                             //Loads this in the stack
            il.Emit(OpCodes.Ldarg_1);                             //Loads first argument (string)
            il.Emit(OpCodes.Ldarg_2);                             //Loads second argument (IDataMapper)
            //Calls AbstractGetter GetConstructor
            il.Emit(OpCodes.Call, typeof(AbstractGetter).GetConstructor(new Type[] { typeof(string), typeof(IDataMapper) }));
            il.Emit(OpCodes.Ret);
        }
        /*
        Builds the type GetDefaultValue Method (Complex)
        */
        private void BuildComplexGetDefaultValue(TypeBuilder getterType)
        {
            //Method Builder
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetDefaultValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[0]);
            ILGenerator il = getValBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldnull);                               //Loads null in the stack
            il.Emit(OpCodes.Ret);
        }
        /*
        Builds the type GetKeytValue Method (Complex)
        */
        private void BuildComplexGetKeyValue(TypeBuilder getterType, PropertyInfo p)
        {
            //Method Builder
            MethodBuilder getValBuilder = getterType.DefineMethod(
                "GetKeyValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(object),
                new Type[] { typeof(object) });
            //Defines this.field db
            FieldInfo field = typeof(AbstractGetter).GetField("db");
            //Defines GetFireKey method from field type   
            MethodInfo GetFireKeyMethod = field.FieldType.GetMethod("GetFireKey", new Type[0]); 
            //Defines get property value method from domain type
            MethodInfo getFieldMethod = domain.GetMethod("get_" + p.Name);  
            //Defines GetValue method from IGetter type
            MethodInfo getValue = typeof(IGetter).GetMethod("GetValue");    
            ILGenerator il = getValBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);                              //Loads this in the stack
            il.Emit(OpCodes.Ldfld, field);                         //Loads field in the stack (db)
            il.Emit(OpCodes.Callvirt, GetFireKeyMethod);           //Calls GetFireKey method
            il.Emit(OpCodes.Ldarg_1);                              //Loads first argument in the stack
            il.Emit(OpCodes.Castclass, domain);                    //Casts the value in top of stack to domain type
            il.Emit(OpCodes.Callvirt, getFieldMethod);             //Calls get property value
            il.Emit(OpCodes.Callvirt, getValue);                   //Calls GetValue method
            if (p.PropertyType.IsPrimitive)                        //Checks if property type is Primitive
            {
                il.Emit(OpCodes.Box, p.PropertyType);              //Boxes the value with the property type
            }
            il.Emit(OpCodes.Ret);
        }
        /*
        Builds the type GetValue Method (Complex)
        */
        private void BuildComplexGetValue(TypeBuilder getterType, PropertyInfo p)
        {
            //Method Builder
            MethodBuilder getValBuilder = getterType.DefineMethod(
                 "GetValue",
                 MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                 typeof(object),
                 new Type[] { typeof(object) });
            //Defines AbstractGetter field db
            FieldInfo field = typeof(AbstractGetter).GetField("db"); 
            ILGenerator il = getValBuilder.GetILGenerator();
            //Defines GetById method from field type (db)
            MethodInfo GetByIdMethod = field.FieldType.GetMethod("GetById", new Type[] { typeof(object) });
            il.Emit(OpCodes.Ldarg_0);                                //Loads this in the stack
            il.Emit(OpCodes.Ldfld, field);                           //Loads field db in the stack
            il.Emit(OpCodes.Ldarg_1);                                //Loads first argument in the stack
            il.Emit(OpCodes.Callvirt, GetByIdMethod);                //Calls GetById method
            il.Emit(OpCodes.Ret);

        }


    }
}