﻿using System;

namespace Woody230.BindableEnum.Library.Models
{
    /// <summary>
    /// Represents an enumeration that is bind safe.
    /// </summary>
    public interface IBindableEnum
    {
        /// <summary>
        /// Whether the enum was able to be binded.
        /// </summary>
        public bool Binded { get; }

        /// <summary>
        /// The enumeration.
        /// </summary>
        public Enum Enum { get; }
    }
}
