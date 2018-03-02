using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace Mtn.Library.Reflection
{
    /// <summary>    
    /// <para>Helper to work with IL.</para>
    /// </summary>
    public class ILHelper
    {
        #region attributes & properties
        private readonly ILGenerator _mIl;
        private List<string> _mVariables;
        private Dictionary<string, Label> _mLabels;
        /// <summary>
        /// <para>Returns the ILGenerator used in Helper .</para>
        /// </summary>
        public ILGenerator IL
        {
            get { return _mIl; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="il">
        /// <para>IL to manipulate.</para>
        /// </param>
        public ILHelper(ILGenerator il)
        {
            _mIl = il;
        }
        #endregion

        #region CreateLocalVar
        /// <summary>
        /// <para>Create a local variable.</para>
        /// </summary>
        /// <param name="name">
        /// <para>Name to the variable.</para>
        /// </param>
        /// <param name="type">
        /// <para>Type of variable.</para>
        /// </param>
        /// <param name="pinned">
        /// <para>Indicates if must  pin the variable in memory.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper CreateLocalVar(string name, Type type, bool pinned)
        {
            if (this._mVariables == null)
                this._mVariables = new List<string>();
            else if (this._mVariables.Contains(name) == true)
                return this;

            this._mVariables.Add(name);
            this._mIl.DeclareLocal(type, pinned);

            return this;
        }
        #endregion

        #region DefineConstructor
        /// <summary>
        /// <para>Define a constructor.</para>
        /// </summary>
        /// <param name="contructor">
        /// <para>ConstructorInfo object to be called.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper DefineConstructor(ConstructorInfo contructor)
        {
            this._mIl.Emit(OpCodes.Call, contructor);
            return this;
        }
        #endregion

        #region InvokeMethod
        /// <summary>
        /// <para>Invoke a method.</para>
        /// </summary>
        /// <param name="methodInfo">
        /// <para>MethodInfo object to be called.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper InvokeMethod(MethodInfo methodInfo)
        {
            this._mIl.EmitCall(OpCodes.Call, methodInfo, null);
            return this;
        }

        /// <summary>
        /// <para>Invoke a method.</para>
        /// </summary>
        /// <param name="type">
        /// <para>Type of the class wich contains the method.</para>
        /// </param>
        /// <param name="name">
        /// <para>Name off method.</para>
        /// </param>
        /// <param name="bindFlag">
        /// <para>Specifies flags that control binding and the way in which the search for members and types is conducted by reflection.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper InvokeMethod(Type type, string name, BindingFlags bindFlag)
        {
            var method = type.GetMethod(name, bindFlag);
            if (method == null)
                throw new Exception("Cannot found method : " + name);

            return InvokeMethod(method);
        }
        /// <summary>
        /// <para>Invoke a method by name and type.</para>
        /// </summary>
        /// <param name="type">
        /// <para>Type of the class wich contains the method.</para>
        /// </param>
        /// <param name="name">
        /// <para>Name off method.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper InvokeMethod(Type type, string name)
        {
            const BindingFlags bindFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            return InvokeMethod(type, name, bindFlag);
        }

        /// <summary>
        /// <para>Emit a Opcode if condition is true.</para>
        /// </summary>
        /// <param name="condition">
        /// <para>Indicates if emit the OpCode or not.</para>
        /// </param>
        /// <param name="opCode">
        /// <para>MSIL instruction</para>
        /// </param>
        /// <param name="value">
        /// <para>Value to be passed.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper EmitIf(bool condition, OpCode opCode, int value)
        {
            if (condition)
                this._mIl.Emit(opCode, value);
            return this;
        }
        #endregion

        #region LoadString
        /// <summary>
        /// <para>Load a string in Stack.</para>
        /// </summary>
        /// <param name="text">
        /// <para>Text to be loaded.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadString(string text)
        {
            this._mIl.Emit(OpCodes.Ldstr, text);
            return this;
        }
        #endregion

        #region Return
        /// <summary>
        /// <para>Emit Return.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper Return()
        {
            this._mIl.Emit(OpCodes.Ret);
            return this;
        }
        #endregion

        #region Try
        /// <summary>
        /// <para>Begin try block (like "try {").</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper BeginTry()
        {
            this._mIl.BeginExceptionBlock();
            return this;
        }
        /// <summary>
        /// <para>Begin try block (like "try {").</para>
        /// </summary>
        /// <param name="label">
        /// <para>Returns Label created.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper BeginTry(out Label label)
        {
            label = this._mIl.BeginExceptionBlock();
            return this;
        }
        /// <summary>
        /// <para>End try block (like "}").</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper EndTry()
        {
            this._mIl.EndExceptionBlock();
            return this;
        }
        /// <summary>
        /// <para>Begin catch block (like "catch {").</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper BeginCatch()
        {
            this._mIl.BeginCatchBlock(typeof(Exception));
            return this;
        }
        /// <summary>
        /// <para>Begin catch block (like "catch {").</para>
        /// </summary>
        /// <param name="type">
        /// <para>Exception type.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper BeginCatch(Type type)
        {
            this._mIl.BeginCatchBlock(type);
            return this;
        }

        /// <summary>
        /// <para>Begin finally block (like "finally {").</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper BeginFinally()
        {
            this._mIl.BeginFinallyBlock();
            return this;
        }
        /// <summary>
        /// <para>Rethrow exception.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper ReThrow()
        {
            this._mIl.Emit(OpCodes.Rethrow);
            return this;
        }
        /// <summary>
        /// <para>Throw exception.</para>
        /// </summary>
        /// <param name="excpType">
        /// <para>Exception type.</para>
        /// </param>
        /// <param name="errorMessage">
        /// <para>Message error.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper Throw(Type excpType, string errorMessage)
        {
            this._mIl.Emit(OpCodes.Ldstr, errorMessage);
            this.NewObj(excpType, typeof(string));
            this._mIl.Emit(OpCodes.Throw);
            return this;
        }
        /// <summary>
        /// <para>Create a new object calling the class constructor.</para>
        /// </summary>
        /// <param name="type">
        /// <para>Type of object.</para>
        /// </param>
        /// <param name="parameters">
        /// <para>Contructor parameters.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper NewObj(Type type, params Type[] parameters)
        {
            this._mIl.Emit(OpCodes.Newobj, type.GetConstructor(parameters));
            return this;
        }
        #endregion

        #region Label
        /// <summary>
        /// <para>Mark a label in code (like ":Label"). Create a new label if don't exist.</para>
        /// </summary>
        /// <param name="name">
        /// <para>Label name.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper MarkLabel(string name)
        {
            if (_mLabels == null || !_mLabels.ContainsKey(name))
                CreateLabel(name);

            if (_mLabels != null)
            {
                Label label = _mLabels[name];

                this._mIl.MarkLabel(label);
            }
            return this;
        }
        /// <summary>
        /// <para>Define a label in code (like ":Label").</para>
        /// </summary>
        /// <param name="name">
        /// <para>Label name.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper CreateLabel(string name)
        {
            if (_mLabels == null)
                _mLabels = new Dictionary<string, Label>();
            else if (_mLabels.ContainsKey(name))
                throw new InvalidOperationException("Label already exist");

            Label lb = this._mIl.DefineLabel();
            _mLabels.Add(name, lb);
            return this;
        }
        #endregion

        #region GotoIfNotNull
        /// <summary>
        /// <para>Goto label if the stack is true or not null.</para>
        /// </summary>
        /// <param name="name">
        /// <para>Label name.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper GotoIfNotNullOrTrue(string name)
        {
            if (this._mLabels == null || !this._mLabels.ContainsKey(name))
                this.CreateLabel(name);

            this._mIl.Emit(OpCodes.Brtrue, _mLabels[name]);
            return this;
        }
        /// <summary>
        /// <para>Goto label if the stack is false or null.</para>
        /// </summary>
        /// <param name="name">
        /// <para>Label name.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper GotoIfNullOrFalse(string name)
        {
            if (this._mLabels == null || !this._mLabels.ContainsKey(name))
                this.CreateLabel(name);

            this._mIl.Emit(OpCodes.Brfalse, _mLabels[name]);
            return this;
        }
        #endregion

        #region Goto
        /// <summary>
        /// <para>Goto label .</para>
        /// </summary>
        /// <param name="name">
        /// <para>Label name.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper Goto(string name)
        {
            if (this._mLabels == null || !this._mLabels.ContainsKey(name))
                this.CreateLabel(name);

            this._mIl.Emit(OpCodes.Br_S, _mLabels[name]);
            return this;
        }
        #endregion
        /// <summary>
        /// <para>Load a Field in Stack.</para>
        /// </summary>
        /// <param name="objectType">
        /// <para>Type of the object wich contains the field.</para>
        /// </param>
        /// <param name="fieldName">
        /// <para>Name of the field.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadField(Type objectType, string fieldName)
        {
            const BindingFlags bindFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = objectType.GetField(fieldName, bindFlag);
            this._mIl.Emit(OpCodes.Ldfld, field);
            return this;
        }
        /// <summary>
        /// <para>Copies a value to memory .</para>
        /// </summary>
        /// <param name="value">
        /// <para>Object value .</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper SetObj(object value)
        {
            this._mIl.Emit(OpCodes.Stobj, value.GetType());
            return this;
        }
        /// <summary>
        /// <para>Load a bool value in Stack.</para>
        /// </summary>
        /// <param name="value">
        /// <para>bool value .</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadBool(bool value)
        {
            this._mIl.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            this.Box(typeof(bool));
            return this;
        }
        /// <summary>
        /// <para>Load a object value in Stack.</para>
        /// </summary>
        /// <param name="value">
        /// <para>Object value .</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadObject(object value)
        {
            if (value is string)
                this.LoadString((string)value);
            else if (value is bool)
                this.LoadBool((bool)value);
            else if (value is int)
            {
                this.LoadInt((int)value);
                //this.Box(typeof(object));
            }
            else
                this.LoadObj(value);

            return this;
        }
        /// <summary>
        /// <para>Load a object value in Stack.</para>
        /// </summary>
        /// <param name="value">
        /// <para>Object value .</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadObj(object value)
        {
            this._mIl.Emit(OpCodes.Ldobj, value.GetType());
            return this;
        }
        /// <summary>
        /// <para>Returns the OpCode by type.</para>
        /// </summary>
        /// <param name="type">
        /// <para>Type of variable.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        private OpCode ReturnOpcodeByType(Type type)
        {
            var ldindOpCodes = new Dictionary<Type, OpCode>();
            ldindOpCodes[typeof(sbyte)] = OpCodes.Ldind_I1;
            ldindOpCodes[typeof(short)] = OpCodes.Ldind_I2;
            ldindOpCodes[typeof(int)] = OpCodes.Ldind_I4;
            ldindOpCodes[typeof(long)] = OpCodes.Ldind_I8;
            ldindOpCodes[typeof(byte)] = OpCodes.Ldind_U1;
            ldindOpCodes[typeof(ushort)] = OpCodes.Ldind_U2;
            ldindOpCodes[typeof(uint)] = OpCodes.Ldind_U4;
            ldindOpCodes[typeof(ulong)] = OpCodes.Ldind_I8;
            ldindOpCodes[typeof(float)] = OpCodes.Ldind_R4;
            ldindOpCodes[typeof(double)] = OpCodes.Ldind_R8;
            ldindOpCodes[typeof(char)] = OpCodes.Ldind_U2;
            ldindOpCodes[typeof(bool)] = OpCodes.Ldind_I1;

            var opCodeObject = ldindOpCodes[type];

            if (opCodeObject != null)
                return opCodeObject;

            return OpCodes.Ldobj;

        }

        #region Box
        /// <summary>
        /// <para>Converts a value type to a object reference .</para>
        /// </summary>
        /// <param name="type">
        /// <para>Type of object.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper Box(Type type)
        {
            if (type.IsValueType)
                this._mIl.Emit(OpCodes.Box, type);
            return this;
        }
        /// <summary>
        /// <para>Converts the boxed representation of type to a unboxed form.</para>
        /// </summary>
        /// <param name="type">
        /// <para>Type of object.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper UnboxAny(Type type)
        {
            if (type.IsValueType)
                this._mIl.Emit(OpCodes.Unbox_Any, type);
            return this;
        }
        #endregion

        #region SetVar
        /// <summary>
        /// <para>Pops the current value to a top in the stack and store in a variable.</para>
        /// </summary>
        /// <param name="name">
        /// <para>Variable name.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper SetVar(string name)
        {
            if (this._mVariables == null)
                throw new InvalidOperationException("Don't exist any variable");

            int position = this._mVariables.IndexOf(name);

            if (position < 0)
                throw new InvalidOperationException("Don't exist variable with this name");

            return this.SetVar(position);
        }
        /// <summary>
        /// <para>Pops the current value to a top in the stack and store in a variable.</para>
        /// </summary>
        /// <param name="position">
        /// <para>Index of variables in stack.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper SetVar(int position)
        {
            switch (position)
            {
                case 0:
                    return this.SetVar0();
                case 1:
                    return this.SetVar1();
                case 2:
                    return this.SetVar2();
                case 3:
                    return this.SetVar3();
            }
            this._mIl.Emit(OpCodes.Stloc, position);
            return this;
        }

        private ILHelper SetVar0()
        {
            this._mIl.Emit(OpCodes.Stloc_0);
            return this;
        }
        private ILHelper SetVar1()
        {
            this._mIl.Emit(OpCodes.Stloc_1);
            return this;
        }
        private ILHelper SetVar2()
        {
            this._mIl.Emit(OpCodes.Stloc_2);
            return this;
        }
        private ILHelper SetVar3()
        {
            this._mIl.Emit(OpCodes.Stloc_3);
            return this;
        }

        #endregion

        #region Load Variables
        /// <summary>
        /// <para>Loads the variable from name to evaluation stack .</para>
        /// </summary>
        /// <param name="name">
        /// <para>Variable name.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadVar(string name)
        {
            if (this._mVariables == null)
                throw new InvalidOperationException("Don't exist any variable");

            int position = this._mVariables.IndexOf(name);

            if (position < 0)
                throw new InvalidOperationException("Don't exist variable with this name");

            return this.LoadVar(position);
        }
        /// <summary>
        /// <para>Loads the variable from name to evaluation stack .</para>
        /// </summary>
        /// <param name="position">
        /// <para>Index of variables in stack.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadVar(int position)
        {
            switch (position)
            {
                case 0:
                    return this.LoadVar0();
                case 1:
                    return this.LoadVar1();
                case 2:
                    return this.LoadVar2();
                case 3:
                    return this.LoadVar3();
            }
            this._mIl.Emit(OpCodes.Ldloc, position);
            return this;
        }

        private ILHelper LoadVar0()
        {
            this._mIl.Emit(OpCodes.Ldloc_0);
            return this;
        }
        private ILHelper LoadVar1()
        {
            this._mIl.Emit(OpCodes.Ldloc_1);
            return this;
        }
        private ILHelper LoadVar2()
        {
            this._mIl.Emit(OpCodes.Ldloc_2);
            return this;
        }
        private ILHelper LoadVar3()
        {
            this._mIl.Emit(OpCodes.Ldloc_3);
            return this;
        }

        #endregion

        #region Load Arguments
        /// <summary>
        /// <para>Loads the argument this (arg 0) onto the stack .</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadThis()
        {
            return this.LoadArgument0();
        }
        /// <summary>
        /// <para>Loads the argument referenced by a specified index (position) onto the stack .</para>
        /// </summary>
        /// <param name="position">
        /// <para>Index of variables in stack.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadArgument(int position)
        {
            switch (position)
            {
                case 0:
                    return this.LoadArgument0();
                case 1:
                    return this.LoadArgument1();
                case 2:
                    return this.LoadArgument2();
                case 3:
                    return this.LoadArgument3();
            }
            this._mIl.Emit(OpCodes.Ldarg, position);
            return this;
        }

        private ILHelper LoadArgument0()
        {
            this._mIl.Emit(OpCodes.Ldarg_0);
            return this;
        }
        private ILHelper LoadArgument1()
        {
            this._mIl.Emit(OpCodes.Ldarg_1);
            return this;
        }
        private ILHelper LoadArgument2()
        {
            this._mIl.Emit(OpCodes.Ldarg_2);
            return this;
        }
        private ILHelper LoadArgument3()
        {
            this._mIl.Emit(OpCodes.Ldarg_3);
            return this;
        }

        #endregion

        #region Arrays
        /// <summary>
        /// <para>Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack .</para>
        /// </summary>
        /// <param name="type">
        /// <para>Type of array.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper ArrayCreate(Type type)
        {
            this._mIl.Emit(OpCodes.Newarr, type);
            return this;
        }
        /// <summary>
        /// <para>Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack .</para>
        /// </summary>
        /// <param name="type">
        /// <para>Type of array.</para>
        /// </param>
        /// <param name="size">
        /// <para>Size of array.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper ArrayCreate(Type type, int size)
        {
            this.LoadInt(size);
            this._mIl.Emit(OpCodes.Newarr, type);
            return this;
        }
        /// <summary>
        /// <para>Replaces the array element at a given index with the object ref value (type O) on the evaluation stack.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper ArrayAdd()
        {
            this._mIl.Emit(OpCodes.Stelem_Ref);
            return this;
        }
        #endregion

        #region Load Int
        /// <summary>
        /// <para>Pushes a supplied value of type int32 onto the evaluation stack as an int32.</para>
        /// </summary>
        /// <param name="value">
        /// <para>Value to push onto the stack.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the ILHelper (object this) with the changes.</para>
        /// </returns>
        public ILHelper LoadInt(int value)
        {
            //if(IntPtr.Size == 8)
            //{
            //    // 64 bit machine
            //    this.m_il.Emit(OpCodes.Ldc_I8, value);
            //}
            //else if(IntPtr.Size == 4)
            //{
            switch (value)
            {
                case -1:
                    return LoadIntM1();
                case 0:
                    return LoadInt0();
                case 1:
                    return LoadInt1();
                case 2:
                    return LoadInt2();
                case 3:
                    return LoadInt3();
                case 4:
                    return LoadInt4();
                case 5:
                    return LoadInt5();
                case 6:
                    return LoadInt6();
                case 7:
                    return LoadInt7();
                case 8:
                    return LoadInt8();
            }

            // 32 bit machine
            this._mIl.Emit(OpCodes.Ldc_I4, value);
            //1} 

            return this;
        }

        #region Aux

        private ILHelper LoadIntM1()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_M1);
            return this;
        }
        private ILHelper LoadInt0()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_0);
            return this;
        }
        private ILHelper LoadInt1()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_1);
            return this;
        }
        private ILHelper LoadInt2()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_2);
            return this;
        }
        private ILHelper LoadInt3()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_3);
            return this;
        }
        private ILHelper LoadInt4()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_4);
            return this;
        }
        private ILHelper LoadInt5()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_5);
            return this;
        }
        private ILHelper LoadInt6()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_6);
            return this;
        }
        private ILHelper LoadInt7()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_7);
            return this;
        }
        private ILHelper LoadInt8()
        {
            this._mIl.Emit(OpCodes.Ldc_I4_8);
            return this;
        }
        #endregion

        #endregion
    }
}
