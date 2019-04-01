using System;
namespace SmartRoadSense.Shared {
    public static class CharacterLevelData {

        static readonly int _startingPoints = 700;
        static readonly int _scalingPoints = 50;
        static readonly int _lostPointsMax = 250;

        public static int PointsToNextLevel(int lvl = -1) {
            if(CharacterManager.Instance.User == null)
                return 0;

            if(lvl == -1)
                lvl = CharacterManager.Instance.User.Level;

            double points = _startingPoints;
            for(var i = 0; i < lvl / 10; i++)
                points += 500 * Math.Pow(2, i);
            points += _scalingPoints * Math.Pow(2, lvl / 10) * (lvl % 10);

            return (int)points;
        }

        public static int CurrentUserLevel() {
            var userLvl = -1;
            while(userLvl < 0) {
                var points = 0;
                for(var i = 1; i <= 100; i++) {
                    points += PointsToNextLevel(i);
                    if(CharacterManager.Instance.User.Experience <= points) {
                        userLvl = i;
                        break;
                    }
                }
            }
            return userLvl;
        }

        public static int CurrentLevelPoints() {
            if(CharacterManager.Instance.User.Level <= 1)
                return CharacterManager.Instance.User.Experience;
            var currentLvlPoints = -1;
            while(currentLvlPoints < 0) {
                var points = 0;
                for(var i = 1; i <= 100; i++) {
                    points += PointsToNextLevel(i);
                    if(CharacterManager.Instance.User.Experience <= points) {
                        currentLvlPoints = CharacterManager.Instance.User.Experience - (points - PointsToNextLevel(i - 1));
                        break;
                    }
                }
            }
            return currentLvlPoints;
        }

        public static int ObtainedPoints(int time) {
            var obtainedPoints = 50;
            var maxBasePoints = 200;
            var bestTimeToBeat = 120000;
            var slowestTimeToBeat = 180000;
            var pointModifier = 0;

            // Scale to difficulty level compared to user level
            /* TODO: reactivate
            var difficultyLevel = LevelManager.Instance.SelectedLevelModel.Difficulty;
            var characterLevel = CharacterManager.Instance.User.Level;
            double pointRatio = difficultyLevel / characterLevel;
            */
            var difficultyLevel = 1;    // TEMP
            double pointRatio = 1;      // TEMP

            if(time <= bestTimeToBeat) {
                // MAX POINTS
                pointModifier = maxBasePoints;
            }
            else if(time < slowestTimeToBeat && time > bestTimeToBeat) {
                // STEPPED POINTS
                var steppedTime = slowestTimeToBeat - bestTimeToBeat;
                var playerTime = steppedTime - (time - bestTimeToBeat);

                pointModifier = playerTime / steppedTime * maxBasePoints;
            }

            obtainedPoints = (int)Math.Round((pointModifier * pointRatio) * (difficultyLevel / 10 + Math.Pow(2, difficultyLevel / 10)));

            return obtainedPoints;
        }

        public static int LostPoints(int position, int trackLength) {
            var difficultyLevel = TrackManager.Instance.SelectedTrackModel.Difficulty;
            var characterLevel = CharacterManager.Instance.User.Level;
            double pointRatio = difficultyLevel / characterLevel;

            var lostPoints = (trackLength - position) / trackLength * _lostPointsMax;
            return -(int)Math.Round((lostPoints * pointRatio) * (difficultyLevel / 10 + Math.Pow(2, difficultyLevel / 10)));
        }
    }
}
