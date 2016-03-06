﻿using Microsoft.CodeAnalysis;
using Pchp.CodeAnalysis.CodeGen;
using Pchp.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Pchp.CodeAnalysis.Semantics
{
    partial class BoundExpression
    {
        internal virtual TypeSymbol Emit(CodeGenerator il)
        {
            throw new NotImplementedException();
        }
    }

    partial class BoundBinaryEx
    {
        internal override TypeSymbol Emit(CodeGenerator il)
        {
            var ltype = Left.Emit(il);
            var rtype = Right.Emit(il);

            if (UsesOperatorMethod)
            {
                throw new NotImplementedException();    // call this.Operator(ltype, rtype)
            }

            switch (this.BinaryOperationKind)
            {
                case Microsoft.CodeAnalysis.Semantics.BinaryOperationKind.OperatorEquals:
                    if (ltype.SpecialType == SpecialType.System_Object && rtype.SpecialType == SpecialType.System_Object)
                    {
                        var ops = il.Routine.DeclaringCompilation.GetTypeByMetadataName("Pchp.Core.Operators");
                        var eqsymbol = ops.GetMembers("Equal").OfType<MethodSymbol>().FirstOrDefault();
                        il.EmitCall(ILOpCode.Call, eqsymbol);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    return (TypeSymbol)il.Routine.DeclaringCompilation.GetSpecialType(SpecialType.System_Boolean);

                default:
                    throw new NotImplementedException();
            }
        }
    }

    partial class BoundLiteral
    {
        internal override TypeSymbol Emit(CodeGenerator il)
        {
            if (!ConstantValue.HasValue)
                throw new InvalidOperationException();

            // TOOD: use ConstantValue

            var value = ConstantValue.Value;
            if (value == null)
            {
                il.IL.EmitNullConstant();
            }
            else
            {
                if (value is int)
                {
                    il.IL.EmitIntConstant((int)value);
                    il.IL.EmitOpCode(System.Reflection.Metadata.ILOpCode.Box);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return (TypeSymbol)il.Routine.DeclaringCompilation.GetSpecialType(SpecialType.System_Object);
        }
    }

    partial class BoundVariableRef
    {
        internal override TypeSymbol Emit(CodeGenerator il)
        {
            if (this.Variable == null)
                throw new InvalidOperationException(); // variable was not resolved

            if (Access == AccessType.Read)
            {
                return this.Variable.GetPlace(il.IL).EmitLoad(il.IL);
            }

            throw new NotImplementedException();
        }
    }
}
