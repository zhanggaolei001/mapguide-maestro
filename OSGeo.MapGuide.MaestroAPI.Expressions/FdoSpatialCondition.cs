﻿#region Disclaimer / License

// Copyright (C) 2015, Jackie Ng
// http://trac.osgeo.org/mapguide/wiki/maestro, jumpinjackie@gmail.com
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//

#endregion Disclaimer / License
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGeo.FDO.Expressions
{
    public enum SpatialOperations
    {
        Contains,
        Crosses,
        Disjoint,
        Equals,
        Intersects,
        Overlaps,
        Touches,
        Within,
        CoveredBy,
        Inside
    }

    public class FdoSpatialCondition : FdoGeometricCondition
    {
        public override FilterType FilterType
        {
            get { return Expressions.FilterType.SpatialCondition; }
        }

        public FdoIdentifier Identifier { get; private set; }

        public SpatialOperations Operator { get; private set; }

        public FdoExpression Expression { get; private set; }

        internal FdoSpatialCondition(ParseTreeNode node)
        {
            this.Identifier = new FdoIdentifier(node.ChildNodes[0]);
            var opName = node.ChildNodes[1].ChildNodes[0].Token.ValueString;
            switch (opName.ToUpper())
            {
                case "CONTAINS":
                    this.Operator = SpatialOperations.Contains;
                    break;
                case "CROSSES":
                    this.Operator = SpatialOperations.Crosses;
                    break;
                case "DISJOINT":
                    this.Operator = SpatialOperations.Disjoint;
                    break;
                case "EQUALS":
                    this.Operator = SpatialOperations.Equals;
                    break;
                case "INTERSECTS":
                    this.Operator = SpatialOperations.Intersects;
                    break;
                case "OVERLAPS":
                    this.Operator = SpatialOperations.Overlaps;
                    break;
                case "TOUCHES":
                    this.Operator = SpatialOperations.Touches;
                    break;
                case "WITHIN":
                    this.Operator = SpatialOperations.Within;
                    break;
                case "COVEREDBY":
                    this.Operator = SpatialOperations.CoveredBy;
                    break;
                case "INSIDE":
                    this.Operator = SpatialOperations.Inside;
                    break;
                default:
                    throw new FdoParseException("Unknown operator: " + opName);
            }
            this.Expression = FdoExpression.ParseNode(node.ChildNodes[2]);
        }
    }
}
