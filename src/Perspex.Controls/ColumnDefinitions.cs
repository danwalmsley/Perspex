﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Globalization;
using System.Linq;
using Perspex.Collections;

namespace Perspex.Controls
{
    /// <summary>
    /// A collection of <see cref="ColumnDefinition"/>s.
    /// </summary>
    public class ColumnDefinitions : PerspexList<ColumnDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinitions"/> class.
        /// </summary>
        public ColumnDefinitions()
        {
            ResetBehavior = ResetBehavior.Remove;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinitions"/> class.
        /// </summary>
        /// <param name="s">A string representation of the column definitions.</param>
        public ColumnDefinitions(string s)
            : this()
        {
            AddRange(GridLength.ParseLengths(s, CultureInfo.InvariantCulture).Select(x => new ColumnDefinition(x)));
        }
    }
}