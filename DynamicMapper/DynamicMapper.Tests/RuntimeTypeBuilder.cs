using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicMapper.Tests
{
    public class RuntimeTypeBuilder
    {
        private const string _moduleName = "DynamicModule";
        #region Properties

        public string Name { get; }
        public IDictionary<string, Type> Properties { get; } = new Dictionary<string, Type>();

        #endregion

        #region Constructors

        public RuntimeTypeBuilder(string typeName)
        {
            Name = typeName;
        }

        #endregion

        #region Methods

        public Type Compile()
        {
            Validate();
            var tb = GetTypeBuilder();
            var constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
            foreach (var field in Properties)
                CreateProperty(tb, field.Key, field.Value);

            var objectType = tb.CreateType();
            return objectType;
        }

        private void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            var fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            var getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            var getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                                MethodAttributes.Public |
                                MethodAttributes.SpecialName |
                                MethodAttributes.HideBySig,
                                null, new[] {propertyType});

            var setIl = setPropMthdBldr.GetILGenerator();
            var modifyProperty = setIl.DefineLabel();
            var exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        private TypeBuilder GetTypeBuilder()
        {
            var assemblyName = new AssemblyName(Name);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(_moduleName);
            var tb = moduleBuilder.DefineType(Name,
                                              TypeAttributes.Public |
                                              TypeAttributes.Class |
                                              TypeAttributes.AutoClass |
                                              TypeAttributes.AnsiClass |
                                              TypeAttributes.BeforeFieldInit |
                                              TypeAttributes.AutoLayout,
                                              null);
            return tb;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException($"{Name} cannot be empty");
            }
            if (!Properties.Any())
            {
                throw new InvalidOperationException("Type cannot have no properties");
            }
        }

        #endregion
    }
}