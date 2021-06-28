/*
 * Copyright (C) 2017-2020. Autumn Beauchesne. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2020
 * 
 * File:    VariantOperand.cs
 * Purpose: Key pair for looking up a variant from a specific table.
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BeauUtil.Variants
{
    /// <summary>
    /// Operand for a variant operation.
    /// </summary>
    [DebuggerDisplay("{ToDebugString()}")]
    public struct VariantOperand : IEquatable<VariantOperand>, IDebugString
    {
        #region Types

        public enum Mode : byte
        {
            Variant,
            TableKey,
            Method
        }

        #endregion // Types

        public readonly Mode Type;
        public readonly Variant Value;
        public readonly TableKeyPair TableKey;
        public readonly MethodCall MethodCall;

        private VariantOperand(Mode inType)
        {
            Type = inType;
            Value = default(Variant);
            TableKey = default(TableKeyPair);
            MethodCall = default(MethodCall);
        }

        public VariantOperand(Variant inValue)
            : this(Mode.Variant)
        {
            Value = inValue;
        }

        public VariantOperand(TableKeyPair inKey)
            : this(Mode.TableKey)
        {
            TableKey = inKey;
        }

        public VariantOperand(MethodCall inMethodCall)
            : this(Mode.Method)
        {
            MethodCall = inMethodCall;
        }

        /// <summary>
        /// Attempts to resolve the value.
        /// </summary>
        public bool TryResolve(IVariantResolver inResolver, object inContext, out Variant outValue, IMethodCache inInvoker = null)
        {
            switch(Type)
            {
                case Mode.TableKey:
                    {
                        return inResolver.TryResolve(inContext, TableKey, out outValue);
                    }

                case Mode.Variant:
                    {
                        outValue = Value;
                        return true;
                    }

                case Mode.Method:
                    {
                        if (inInvoker == null)
                            throw new ArgumentNullException("inInvoker", "No IMethodCache provided - cannot invoke a method call operand");
                        
                        object obj;
                        if (!inInvoker.TryStaticInvoke(MethodCall, inContext, out obj) || !Variant.TryConvertFrom(obj, out outValue))
                        {
                            outValue = default(Variant);
                            return false;
                        }

                        return true;
                    }

                default:
                    throw new InvalidOperationException("Unknown operand type " + Type.ToString());
            }
        }
    
        #region IEquatable

        public bool Equals(VariantOperand inOther)
        {
            if (Type != inOther.Type)
                return false;

            switch(Type)
            {
                case Mode.TableKey:
                    return TableKey.Equals(inOther.TableKey);
                case Mode.Variant:
                    return Value.StrictEquals(inOther.Value);
                case Mode.Method:
                    return MethodCall.Equals(inOther.MethodCall);
            }

            return true;
        }

        #endregion // IEquatable

        #region Overrides

        public override int GetHashCode()
        {
            int hash = Type.GetHashCode();

            switch(Type)
            {
                case Mode.Variant:
                    hash = (hash << 5) ^ Value.GetHashCode();
                    break;

                case Mode.TableKey:
                    hash = (hash << 5) ^ TableKey.GetHashCode();
                    break;

                case Mode.Method:
                    hash = (hash << 5) ^ MethodCall.GetHashCode();
                    break;
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is VariantOperand)
                return Equals((VariantOperand) obj);

            return false;
        }

        public override string ToString()
        {
            switch(Type)
            {
                case Mode.Variant:
                    return Value.ToString();

                case Mode.TableKey:
                    return TableKey.ToString();

                case Mode.Method:
                    return MethodCall.ToString();

                default:
                    throw new InvalidOperationException("Unknown variant operand type " + Type.ToString());
            }
        }

        public string ToDebugString()
        {
            switch(Type)
            {
                case Mode.Variant:
                    return Value.ToDebugString();

                case Mode.TableKey:
                    return TableKey.ToDebugString();

                case Mode.Method:
                    return MethodCall.ToDebugString();

                default:
                    throw new InvalidOperationException("Unknown variant operand type " + Type.ToString());
            }
        }

        #endregion // Overrides
    
        #region Parse

        /// <summary>
        /// Attempts to parse data into an operand.
        /// </summary>
        static public bool TryParse(StringSlice inData, out VariantOperand outOperand)
        {
            Variant value;
            if (Variant.TryParse(inData, out value))
            {
                outOperand = new VariantOperand(value);
                return true;
            }

            TableKeyPair tableKey;
            if (TableKeyPair.TryParse(inData, out tableKey))
            {
                outOperand = new VariantOperand(tableKey);
                return true;
            }

            MethodCall call;
            if (MethodCall.TryParse(inData, out call))
            {
                outOperand = new VariantOperand(call);
                return true;
            }

            outOperand = default(VariantOperand);
            return false;
        }

        #endregion // Parse
    }
}