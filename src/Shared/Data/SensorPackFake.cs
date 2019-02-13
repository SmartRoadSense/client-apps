using System;
using System.Collections.Generic;
using System.Text;

using SmartRoadSense.Core;

namespace SmartRoadSense.Shared.Data {

    /// <summary>
    /// Fake sensor pack that does not collect sensor data.
    /// </summary>
    public class SensorPackFake : SensorPack {

        public SensorPackFake(Engine engine)
            : base(engine) {
        }

        protected override void StartSensingCore() {
        }

        protected override void StopSensingCore() {
        }

    }

}
