using System;
namespace SmartRoadSense.Shared {
    public class CrossSettingsIdentifiers {
        public string Value;

        CrossSettingsIdentifiers(string value) { Value = value; }

        // VEHICLE DATA
        public static CrossSettingsIdentifiers VehicleCount => new CrossSettingsIdentifiers("VEHICLES_COUNT");
        public static CrossSettingsIdentifiers IdVehicle => new CrossSettingsIdentifiers("VEHICLES_ID_VEHICLE");
        public static CrossSettingsIdentifiers SelectedGarageVehicle => new CrossSettingsIdentifiers("VEHICLES_SELECTED_GARAGE_VEHICLE");
        public static CrossSettingsIdentifiers SelectedVehicle => new CrossSettingsIdentifiers("VEHICLES_SELECTED_VEHICLE");

        // VEHICLE UPGRADES
        /*
        public static CrossSettingsIdentifiers SelectedVehiclePerformance => new CrossSettingsIdentifiers("VEHICLES_SELECTED_VEHICLE_PERFORMANCE");
        public static CrossSettingsIdentifiers SelectedVehicleWheels => new CrossSettingsIdentifiers("VEHICLES_SELECTED_VEHICLE_WHEELS");
        public static CrossSettingsIdentifiers SelectedVehicleSuspensions => new CrossSettingsIdentifiers("VEHICLES_SELECTED_VEHICLE_SUSPENSIONS");
        public static CrossSettingsIdentifiers SelectedVehicleBrakes => new CrossSettingsIdentifiers("VEHICLES_SELECTED_VEHICLE_BRAKES");
        */
        // USER DATA
        public static CrossSettingsIdentifiers UserInfo => new CrossSettingsIdentifiers("USER_INFO");
        public static CrossSettingsIdentifiers CharacterCount => new CrossSettingsIdentifiers("CHARACTERS_COUNT");
        public static CrossSettingsIdentifiers IdCharacter => new CrossSettingsIdentifiers("CHARACTERS_ID_CHARACTER");
        public static CrossSettingsIdentifiers SelectedUserCharacter => new CrossSettingsIdentifiers("CHARACTERS_SELECTED_USER_CHARACTER");
        public static CrossSettingsIdentifiers SelectedCharacter => new CrossSettingsIdentifiers("CHARACTERS_SELECTED_CHARACTER");

        //LEVEL DATA
        public static CrossSettingsIdentifiers Levels => new CrossSettingsIdentifiers("LEVEL_LEVELS");
        public static CrossSettingsIdentifiers LevelInfo => new CrossSettingsIdentifiers("LEVEL_INFO");
        public static CrossSettingsIdentifiers LevelCount => new CrossSettingsIdentifiers("LEVELS_COUNT");
        public static CrossSettingsIdentifiers IdLevel => new CrossSettingsIdentifiers("LEVELS_ID_LEVEL");
        public static CrossSettingsIdentifiers SelectedLevel => new CrossSettingsIdentifiers("LEVELS_SELECTED_LEVEL");
        public static CrossSettingsIdentifiers LastPlayedLevel => new CrossSettingsIdentifiers("LEVELS_LAST_PLAYED");

        //SETTINGS
        public static CrossSettingsIdentifiers ButtonDimension => new CrossSettingsIdentifiers("BUTTONS_DIMENSION");
        public static CrossSettingsIdentifiers ButtonOrientation => new CrossSettingsIdentifiers("BUTTONS_ORIENTATION");

        // SOUNDS
        public static CrossSettingsIdentifiers Music => new CrossSettingsIdentifiers("SOUND_MUSIC");
        public static CrossSettingsIdentifiers SoundEffects => new CrossSettingsIdentifiers("SOUND_EFFECTS");
    }
}
