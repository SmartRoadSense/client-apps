using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Toolkit.Parameters;

namespace SmartRoadSense.Toolkit.Generators {

    abstract class BaseGenerator {

        /// <summary>
        /// Factory method for generator instantiation.
        /// </summary>
        public static BaseGenerator Create(CommonParameters parameters) {
            BaseGenerator ret = null;

            if(parameters.SourceFiles != null && parameters.SourceFiles.Count() > 0) {
                ret = new FileSequenceGenerator();
            }
            else if(parameters.RandomGenerator) {
                ret = new RandomGenerator();
            }

            if (ret != null) {
                ret.Parameters = parameters;
            }

            return ret;
        }

        /// <summary>
        /// Application parameters used by generator.
        /// </summary>
        protected CommonParameters Parameters { get; private set; }

        /// <summary>
        /// Generates an enumeration of data pieces.
        /// </summary>
        public abstract IEnumerable<DataPiece> Generate();

    }

}
