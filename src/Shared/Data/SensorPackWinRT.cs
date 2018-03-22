#if WINDOWS_PHONE_APP
using System;
using System.Collections.Generic;
using System.Text;

using SmartRoadSense.Core;

namespace SmartRoadSense.Shared.Data {

    public class SensorPackWinRT : SensorPack {

        public SensorPackWinRT(Engine engine)
            : base(engine) {

        }

        protected override void StartSensingCore() {
            
        }

        protected override void StopSensingCore() {
            
        }

    }

}

#endif
