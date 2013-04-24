// Copyright 2010-2013 Chris Patterson
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Stact.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using Internals.Caching;
    using Internals.Extensions;


    public class DynamicImplementationBuilder :
        ImplementationBuilder
    {
        const MethodAttributes PropertyAccessMethodAttributes = MethodAttributes.Public
                                                                | MethodAttributes.SpecialName
                                                                | MethodAttributes.HideBySig
                                                                | MethodAttributes.Final
                                                                | MethodAttributes.Virtual
                                                                | MethodAttributes.VtableLayoutMask;

        readonly Cache<string, ModuleBuilder> _moduleBuilders;
        readonly string _proxyNamespaceSuffix = ".DynamicImpl" + Guid.NewGuid().ToString("N");
        readonly Cache<Type, Type> _proxyTypes;

        public DynamicImplementationBuilder()
        {
            _moduleBuilders = new ConcurrentCache<string, ModuleBuilder>();

            _proxyTypes = new ConcurrentCache<Type, Type>((MissingValueProvider<Type, Type>)(CreateImplementation));
        }

        public Type GetImplementationType(Type interfaceType)
        {
            return _proxyTypes.Get(interfaceType, CreateImplementation);
        }

        Type CreateImplementation(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("Proxies can only be created for interfaces: " + interfaceType.Name,
                    "interfaceType");
            }

            Type implementationType = null;
            GetModuleBuilderForType(interfaceType,
                moduleBuilder => { implementationType = CreateTypeFromInterface(moduleBuilder, interfaceType); });

            return implementationType;
        }

        Type CreateTypeFromInterface(ModuleBuilder builder, Type interfaceType)
        {
            string typeName = interfaceType.Namespace + _proxyNamespaceSuffix + "." +
                              (interfaceType.IsNested
                                   ? (interfaceType.DeclaringType.Name + '+' + interfaceType.Name)
                                   : interfaceType.Name);
            try
            {
                TypeBuilder typeBuilder = builder.DefineType(typeName,
                    TypeAttributes.Serializable | TypeAttributes.Class |
                    TypeAttributes.Public | TypeAttributes.Sealed,
                    typeof(object), new[] {interfaceType});

                typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

                IEnumerable<PropertyInfo> properties = interfaceType.GetAllProperties();
                foreach (PropertyInfo property in properties)
                {
                    FieldBuilder fieldBuilder = typeBuilder.DefineField("field_" + property.Name, property.PropertyType,
                        FieldAttributes.Private);

                    PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name,
                        property.Attributes | PropertyAttributes.HasDefault, property.PropertyType, null);

                    MethodBuilder getMethod = GetGetMethodBuilder(property, typeBuilder, fieldBuilder);
                    MethodBuilder setMethod = GetSetMethodBuilder(property, typeBuilder, fieldBuilder);

                    propertyBuilder.SetGetMethod(getMethod);
                    propertyBuilder.SetSetMethod(setMethod);
                }

                return typeBuilder.CreateType();
            }
            catch (Exception ex)
            {
                string message = string.Format("Exception creating proxy ({0}) for {1}", typeName,
                    interfaceType.GetTypeName());
                throw new StactException(message, ex);
            }
        }

        MethodBuilder GetGetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder,
            FieldBuilder fieldBuilder)
        {
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyInfo.Name,
                PropertyAccessMethodAttributes,
                propertyInfo.PropertyType,
                Type.EmptyTypes);

            ILGenerator il = getMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        MethodBuilder GetSetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder,
            FieldBuilder fieldBuilder)
        {
            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyInfo.Name,
                PropertyAccessMethodAttributes,
                null,
                new[] {propertyInfo.PropertyType});

            ILGenerator il = setMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }

        void GetModuleBuilderForType(Type interfaceType, Action<ModuleBuilder> callback)
        {
            string assemblyName = interfaceType.Namespace + _proxyNamespaceSuffix;

            ModuleBuilder builder = _moduleBuilders.Get(assemblyName, name =>
                {
                    AssemblyBuilder assemblyBuilder = AppDomain
                        .CurrentDomain
                        .DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndCollect);

                    ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

                    return moduleBuilder;
                });

            callback(builder);
        }
    }
}