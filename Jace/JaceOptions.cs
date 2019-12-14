using Jace.Execution;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Jace
{
    public class JaceOptions
    {
        internal const int DefaultCacheMaximumSize = 500;
        internal const int DefaultCacheReductionSize = 50;

        public JaceOptions()
        {
            CultureInfo = CultureInfo.CurrentCulture;
            ExecutionMode = ExecutionMode.Compiled;
            CacheEnabled = true;
            OptimizerEnabled = true;
            CaseSensitive = false;
            DefaultFunctions = true;
            DefaultConstants = true;
            CacheMaximumSize = DefaultCacheMaximumSize;
            CacheReductionSize = DefaultCacheReductionSize;
        }

        /// <summary>
        /// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The execution mode that must be used for formula execution.
        /// </summary>
        public ExecutionMode ExecutionMode { get; set; }

        /// <summary>
        /// Enable or disable caching of mathematical formulas.
        /// </summary>
        public bool CacheEnabled { get; set; }

        /// <summary>
        /// Configure the maximum cache size for mathematical formulas.
        /// </summary>
        public int CacheMaximumSize { get; set; }

        /// <summary>
        /// Configure the cache reduction size for mathematical formulas.
        /// </summary>
        public int CacheReductionSize { get; set; }

        /// <summary>
        /// Enable or disable optimizing of formulas.
        /// </summary>
        public bool OptimizerEnabled { get; set; }

        /// <summary>
        /// Enable or disable converting to lower case. This parameter is the inverse of <see cref="CaseSensitive"/>.
        /// </summary>
        [Obsolete]
        public bool AdjustVariableCase { 
            get
            {
                return !CaseSensitive;
            }
            set
            {
                CaseSensitive = !value;
            }
        }

        /// <summary>
        /// Enable case sensitive or case insensitive processing mode.
        /// </summary>
        public bool CaseSensitive { get;  set; }

        /// <summary>
        /// Enable or disable the default functions.
        /// </summary>
        public bool DefaultFunctions { get; set; }

        /// <summary>
        /// Enable or disable the default constants.
        /// </summary>
        public bool DefaultConstants { get; set; }
    }
}
