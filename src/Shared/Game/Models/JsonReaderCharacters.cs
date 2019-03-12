using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {
    public static class JsonReaderCharacter {
        const string CharacterJsonFilename = "Characters.json";

        static CharacterContainerModel characterContainer = null;

        static void LoadConfig() 
        {
            if(characterContainer != null)
                return;
               
            try {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(App));
                string[] resourceNames = assembly.GetManifestResourceNames();
                var fullname = (from r in resourceNames where r.EndsWith(CharacterJsonFilename, StringComparison.Ordinal) select r).FirstOrDefault();

                using(var s = assembly.GetManifestResourceStream(fullname)) {
                    using(var reader = new System.IO.StreamReader(s)) {
                        var txt = reader.ReadToEnd();
                        JsonSerializer serializer = new JsonSerializer();
                        characterContainer = JsonConvert.DeserializeObject<CharacterContainerModel>(txt);
                    }
                }
            }
            catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Error decoding character file: " + e);
            }
        }

        public static void GetCharacterConfig() 
        {
            LoadConfig();
            if(characterContainer != null) 
            {
                System.Diagnostics.Debug.WriteLine("character count: {0}", characterContainer.CharacterModel.Count);
                CharacterManager.Instance.CharacterCount = characterContainer.CharacterModel.Count;
            }
        }

        public static CharacterModel GetSingleCharacter(int id) 
        {
            LoadConfig();
            var character = characterContainer.CharacterModel.FirstOrDefault(characterContainer => characterContainer.IdCharacter == id);
            return character;

            //CharacterManager.Instance.SelectedCharacterModel = character;   
            //System.Diagnostics.Debug.WriteLine("id: " + character.IdCharacter);
            //System.Diagnostics.Debug.WriteLine("type: " + character.Type);
            //System.Diagnostics.Debug.WriteLine("image position: {0},{1},{2},{3}", character.ImagePosition.Bottom, character.ImagePosition.Top, character.ImagePosition.Left, character.ImagePosition.Right);

        }
    }
}
