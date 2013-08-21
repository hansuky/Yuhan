using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;

//  http://www.codeproject.com/useritems/Dynamic_Code_Generation.asp
//  http://www.ozcandegirmenci.com/post/2008/02/Create-object-instances-Faster-than-Reflection.aspx


namespace Yuhan.WPF.DsxGridCtrl
{
    public delegate object  GetHandler(object source);
    public delegate void    SetHandler(object source, object value);
    public delegate object  CreateHandler();

    public sealed class MethodCompiler
    {
        #region ctors

        private MethodCompiler() 
        { 
        }

        #endregion

        #region Method - CreateNewHandler

        // CreateInstantiateObjectDelegate
        public static CreateHandler CreateNewHandler(Type type)
        {
            ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                throw new ApplicationException(string.Format("The type {0} must declare an empty constructor (the constructor may be private, internal, protected, protected internal, or public).", type));
            }

            DynamicMethod   _dynamic = new DynamicMethod(string.Empty, typeof(object), new Type[0], constructorInfo.DeclaringType);
            ILGenerator     _il      = _dynamic.GetILGenerator();
            _il.DeclareLocal(constructorInfo.DeclaringType);
            _il.Emit(OpCodes.Newobj, constructorInfo);
            _il.Emit(OpCodes.Stloc_0);
            _il.Emit(OpCodes.Ldloc_0);
            _il.Emit(OpCodes.Ret);

            return (CreateHandler)_dynamic.CreateDelegate(typeof(CreateHandler));
        }
        #endregion
        

        #region Method - CreateGetHandler (property)

        // CreateGetDelegate
        public static GetHandler CreateGetHandler(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Call, getMethodInfo);
            BoxIfNeeded(getMethodInfo.ReturnType, getGenerator);
            getGenerator.Emit(OpCodes.Ret);

            return (GetHandler)dynamicGet.CreateDelegate(typeof(GetHandler));
        }
        #endregion

        #region Method - CreateGetHandler (field)

        // CreateGetDelegate
        public static GetHandler CreateGetHandler(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, fieldInfo);
            BoxIfNeeded(fieldInfo.FieldType, getGenerator);
            getGenerator.Emit(OpCodes.Ret);

            return (GetHandler)dynamicGet.CreateDelegate(typeof(GetHandler));
        }
        #endregion

        #region Method - CreateSetHandler (property)

        private static Dictionary<string, SetHandler> s_SetHandlerCache = new Dictionary<string, SetHandler>();

        // CreateSetDelegate
        public static SetHandler CreateSetHandler(Type type, PropertyInfo propertyInfo)
        {
            string _key = string.Format("{0}.{1}", propertyInfo.DeclaringType, propertyInfo.ToString());
            
            if (s_SetHandlerCache.ContainsKey(_key))
            {
                return s_SetHandlerCache[_key] as SetHandler;
            }

            MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            UnboxIfNeeded(setMethodInfo.GetParameters()[0].ParameterType, setGenerator);
            setGenerator.Emit(OpCodes.Call, setMethodInfo);
            setGenerator.Emit(OpCodes.Ret);

            SetHandler _newHandler =  (SetHandler)dynamicSet.CreateDelegate(typeof(SetHandler));
            s_SetHandlerCache.Add(_key, _newHandler);
            return _newHandler;
        }
        #endregion

        #region Method - CreateSetHandler (field)

        // CreateSetDelegate
        public static SetHandler CreateSetHandler(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            UnboxIfNeeded(fieldInfo.FieldType, setGenerator);
            setGenerator.Emit(OpCodes.Stfld, fieldInfo);
            setGenerator.Emit(OpCodes.Ret);

            return (SetHandler)dynamicSet.CreateDelegate(typeof(SetHandler));
        }
        #endregion


        #region Method - CreateGetDynamicMethod

        private static DynamicMethod CreateGetDynamicMethod(Type type)
        {
            return new DynamicMethod("DynamicGet", typeof(object), new Type[] { typeof(object) }, type, true);
        }
        #endregion

        #region Method - CreateSetDynamicMethod

        private static DynamicMethod CreateSetDynamicMethod(Type type)
        {
            return new DynamicMethod("DynamicSet", typeof(void), new Type[] { typeof(object), typeof(object) }, type, true);
        }
        #endregion

        #region Method - BoxIfNeeded

        private static void BoxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }
        }
        #endregion

        #region Method - UnboxIfNeeded

        private static void UnboxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, type);
            }
        }
        #endregion
    }
}