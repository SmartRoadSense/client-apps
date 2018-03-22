﻿using System;

namespace SmartRoadSense.Shared.Data {

    /// <summary>
    /// Information about a recording session.
    /// </summary>
    public class SessionInfo {

        public SessionInfo(VehicleType vehicle, AnchorageType anchorage, int numberOfPeople) {
            _trackId = Guid.NewGuid();
            _startingTimestamp = DateTime.UtcNow;
            _vehicle = vehicle;
            _anchorage = anchorage;
            _numberOfPeople = numberOfPeople;
        }

        private readonly Guid _trackId;

        /// <summary>
        /// Gets an unique ID that identifies the track generated by the current session.
        /// </summary>
        public Guid TrackId {
            get {
                return _trackId;
            }
        }

        private readonly DateTime _startingTimestamp;

        /// <summary>
        /// Gets when the current recording session was started, in UTC.
        /// </summary>
        public DateTime StartTimestampUtc {
            get {
                return _startingTimestamp;
            }
        }

        private readonly VehicleType _vehicle;

        // TODO: value must be updated when changed by user
        public VehicleType Vehicle {
            get {
                return _vehicle;
            }
        }

        private readonly AnchorageType _anchorage;

        // TODO: value must be updated when changed by user
        public AnchorageType Anchorage {
            get {
                return _anchorage;
            }
        }

        private readonly int _numberOfPeople;

        // TODO: value must be updated when changed by user
        public int NumberOfPeople {
            get {
                return _numberOfPeople;
            }
        }

        private double _minPpe = double.MaxValue;
        private double _maxPpe = double.MinValue;
        private double _currentPpe = double.NaN;

        public double MinimumMeasurement {
            get {
                return _minPpe;
            }
        }

        public double MaximumMeasurement {
            get {
                return _maxPpe;
            }
        }

        public double LastMeasurement {
            get {
                return _currentPpe;
            }
        }

        public void NewMeasurement(double value) {
            _currentPpe = value;

            if (value > _maxPpe)
                _maxPpe = value;
            if (value < _minPpe)
                _minPpe = value;
        }

    }

}

