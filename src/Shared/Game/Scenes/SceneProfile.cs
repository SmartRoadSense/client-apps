using System;
using Urho;
using Urho.Gui;
using Urho.Resources;


namespace SmartRoadSense.Shared {
    public class SceneProfile : BaseScene {
        ScreenInfoRatio dim; //variabile rapporto dimensioni schermo
        ResourceCache cache;
        Sprite backgroundSprite;
        Sprite black_bar;
        Sprite cont_profile;
        Sprite cont_img;
        UIElement root;
        Button male;
        Button female;
        Button other;
        Button p_prev_img;
        Button n_next_img;
        Button prev_img;
        Button next_img;
        Button sel_img;
        Text screen_info;
        UI ui;
        Font font;

        int counter;
        string _nameText;
        bool _isUserProfile;
        LineEdit lineEdit;

        public SceneProfile(Game game, bool IsUserProfile = false) : base(game) {
            dim = GameInstance.ScreenInfo;
            root = GameInstance.UI.Root;
            cache = GameInstance.ResourceCache;
            font = cache.GetFont("Fonts/OpenSans-Bold.ttf");
            ui = GameInstance.UI;
            _isUserProfile = IsUserProfile;
            JsonReaderCharacter.GetCharacterConfig();
            counter = CharacterManager.Instance.User != null ? CharacterManager.Instance.User.PortraitId : 0;
            _nameText = CharacterManager.Instance.User?.Username;
           
            CreateUI();
        }

        public void CreateUI() {
            CreateBackground();
            CreateTopBar();
            CreateProfileBar();
            ScrollImage();
        }

        void CreateBackground() {
            //TODO: animated background
            var backgroundTexture = cache.GetTexture2D("Textures/MenuBackground.png");
            if(backgroundTexture == null)
                return;
            backgroundSprite = root.CreateSprite();
            backgroundSprite.Texture = backgroundTexture;
            backgroundSprite.SetSize((int)(dim.XScreenRatio * 1920), (int)(dim.YScreenRatio * 1080));
            backgroundSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            backgroundSprite.SetPosition(0, 0);
        }

        void CreateTopBar() {
            var generic_bts = cache.GetTexture2D("Textures/Generic/generic_btn.png");

            black_bar = root.CreateSprite();
            root.AddChild(black_bar);
            black_bar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);
            black_bar.Opacity = 0.5f;
            black_bar.SetPosition(0, (int)(dim.YScreenRatio * 30));
            black_bar.SetSize((int)(dim.XScreenRatio * 2000), (int)(dim.YScreenRatio * 140));
            black_bar.ImageRect = AssetsCoordinates.Generic.TopBar.Rectangle;

            // BACK
            Button btn_back = new Button();
            root.AddChild(btn_back);
            btn_back.SetStyleAuto(null);
            btn_back.SetPosition((int)(dim.XScreenRatio * 40), (int)(dim.YScreenRatio * 40));
            btn_back.SetSize((int)(dim.XScreenRatio * 120), (int)(dim.YScreenRatio * 120));
            btn_back.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btn_back.ImageRect = AssetsCoordinates.Generic.Icons.BntBack;
            btn_back.Pressed += args => {
                if(_isUserProfile)
                    GameInstance.LaunchScene(GameScenesEnumeration.USER_PROFILE);
                else
                    GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            };

            //COINS
            Button coins = new Button();
            root.AddChild(coins);
            coins.SetStyleAuto(null);
            coins.SetPosition((int)(dim.XScreenRatio * 165), (int)(dim.YScreenRatio * 60));
            coins.SetSize((int)(dim.XScreenRatio * 70), (int)(dim.YScreenRatio * 70));
            coins.Texture = generic_bts;
            coins.ImageRect = new IntRect(60, 729, 110, 779);
            coins.Visible = false;

            //Wallet text
            Text wallet = new Text();
            root.AddChild(wallet);
            wallet.SetPosition((int)(dim.XScreenRatio * 250), (int)(dim.YScreenRatio * 70));
            wallet.SetFont(font, dim.XScreenRatio * 30);
            int wallet_tot = CharacterManager.Instance.Wallet;
            wallet.Value = "" + wallet_tot;
            wallet.Visible = false;

            // SCREEN TITLE
            Button screen_title = new Button();
            root.AddChild(screen_title);
            screen_title.SetStyleAuto(null);
            screen_title.SetPosition((int)(dim.XScreenRatio * 1500), (int)(dim.YScreenRatio * 50));
            screen_title.SetSize((int)(dim.XScreenRatio * 400), (int)(dim.YScreenRatio * 100));
            screen_title.Texture = generic_bts;
            screen_title.Enabled = false;
            screen_title.ImageRect = new IntRect(0, 110, 513, 213);
            Text buttonTitleText = new Text();
            screen_title.AddChild(buttonTitleText);
            buttonTitleText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonTitleText.SetPosition(0, 0);
            buttonTitleText.SetFont(font, dim.XScreenRatio * 30);
            buttonTitleText.Value = "CHARACTER";

            screen_info = new Text();
            screen_title.AddChild(screen_info);
            screen_info.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            screen_info.SetPosition((int)(dim.XScreenRatio * -100), (int)(dim.YScreenRatio * 80));
            screen_info.SetFont(font, dim.XScreenRatio * 20);
            screen_info.SetColor(Color.White);
            //screen_info.Value = "Please enter a name and select an character.";
            screen_info.Value = "Please select a character.";
        }

        void CreateProfileBar() {
            var generic_bts = cache.GetTexture2D("Textures/Generic/generic_btn.png");
            var cont_base = cache.GetTexture2D("Textures/Garage/cont_base.png");
            var profiles = cache.GetTexture2D("Textures/Generic/profiles.png");
            var garage_bts = cache.GetTexture2D("Textures/Garage/garage_bts.png");

            // Buttons container (root element)
            cont_profile = root.CreateSprite();
            cont_profile.Texture = cont_base;
            cont_profile.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 600));
            cont_profile.SetPosition((int)(dim.XScreenRatio * 10), (int)(dim.YScreenRatio * 160));
            cont_profile.ImageRect = new IntRect(0, 0, 56, 56);

            // Continue
            var continueBtn = new Button();
            root.AddChild(continueBtn);
            continueBtn.SetStyleAuto(null);
            continueBtn.SetPosition(dim.SetX(0), dim.SetY(-40));
            continueBtn.HorizontalAlignment = HorizontalAlignment.Center;
            continueBtn.VerticalAlignment = VerticalAlignment.Bottom;
            continueBtn.SetSize(dim.SetX(300), dim.SetY(100));
            continueBtn.Texture = generic_bts;
            continueBtn.ImageRect = new IntRect(260, 0, 520, 110);

            Text continueText = new Text();
            continueBtn.AddChild(continueText);
            continueText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            continueText.SetPosition(0, 0);
            continueText.SetFont(font, dim.XScreenRatio * 35);
            continueText.Value = "Continue";

            continueBtn.Pressed += (PressedEventArgs args) => {
                System.Diagnostics.Debug.WriteLine("Pressed");
                if(CharacterManager.Instance.User == null) {
                    // Creating new user
                    System.Diagnostics.Debug.WriteLine("Creating new user");
                    CharacterManager.Instance.User = new UserInfo {
                        Experience = 0,
                        PortraitId = counter,
                        Username = _nameText,
                        Wallet = 0
                    };
                }
                else {
                    // Update user data
                    System.Diagnostics.Debug.WriteLine("Updating user");
                    var user = CharacterManager.Instance.User;
                    user.PortraitId = counter;
                    user.Username = _nameText;
                    CharacterManager.Instance.User = user;
                }
                if(_isUserProfile)
                    GameInstance.LaunchScene(GameScenesEnumeration.USER_PROFILE);
                else
                    GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            };

            Window nameContainer = new Window();
            cont_profile.AddChild(nameContainer);
            nameContainer.SetStyleAuto(null);
            nameContainer.SetPosition((int)(dim.XScreenRatio * 100), (int)(dim.YScreenRatio * 50));
            nameContainer.SetSize((int)(dim.XScreenRatio * 650), (int)(dim.YScreenRatio * 100));
            nameContainer.Texture = generic_bts;
            nameContainer.ImageRect = new IntRect(0, 288, 1012, 402);
            continueBtn.Visible = !string.IsNullOrEmpty(_nameText);

            var enterTextString = "Rudy Reckless";
            if(_isUserProfile)
                enterTextString = CharacterManager.Instance.User.Username;

            lineEdit = new LineEdit {
                Name = "nameEdit",
                Position = new IntVector2(dim.SetX(0), dim.SetY(0)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Size = new IntVector2(dim.SetX(640), dim.SetY(90))
            };
            lineEdit.SetColor(Color.Transparent);
            lineEdit.Text = enterTextString;
            lineEdit.TextElement.SetFont(font, dim.SetX(35));
            lineEdit.TextElement.VerticalAlignment = VerticalAlignment.Center;
            lineEdit.TextElement.HorizontalAlignment = HorizontalAlignment.Center;
            nameContainer.AddChild(lineEdit);

            continueBtn.Visible = true;
            lineEdit.Focused += (FocusedEventArgs args) => {
                _nameText = enterTextString;
                System.Diagnostics.Debug.WriteLine("Focused line edit");
            };

            /*
            lineEdit.TextChanged += (TextChangedEventArgs args) => {
                if(args.Text.Equals(enterTextString) || args.Text.Contains("Enter n")) {
                    lineEdit.Text = string.Empty;
                    _nameText = string.Empty;
                    continueBtn.Visible = false;
                    return;
                }
                if(args.Text.Length < 3) {
                    continueBtn.Visible = false;
                    return;
                }
                continueBtn.Visible = true;
                _nameText = args.Text;
            };
            */

            //male
            male = new Button();
            cont_profile.AddChild(male);
            male.SetStyleAuto(null);
            male.SetPosition((int)(dim.XScreenRatio * 800), (int)(dim.YScreenRatio * 50));
            male.SetSize((int)(dim.XScreenRatio * 300), (int)(dim.YScreenRatio * 100));
            male.Texture = generic_bts;
            male.ImageRect = new IntRect(0, 0, 260, 110);
            Text buttonmale = new Text();
            male.AddChild(buttonmale);
            buttonmale.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonmale.SetPosition(0, 0);
            buttonmale.SetFont(font, dim.XScreenRatio * 30);
            buttonmale.Value = "MALE";
            male.Pressed += args => {
                counter = 0;
                ScrollImage();
             };

            //female
            female = new Button();
            cont_profile.AddChild(female);
            female.SetStyleAuto(null);
            female.SetPosition((int)(dim.XScreenRatio * 1120), (int)(dim.YScreenRatio * 50));
            female.SetSize((int)(dim.XScreenRatio * 300), (int)(dim.YScreenRatio * 100));
            female.Texture = generic_bts;
            female.ImageRect = new IntRect(0, 0, 260, 110);
            Text buttonfemale = new Text();
            female.AddChild(buttonfemale);
            buttonfemale.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonfemale.SetPosition(0, 0);
            buttonfemale.SetFont(font, dim.XScreenRatio * 30);
            buttonfemale.Value = "FEMALE";
            female.Pressed += args => {
                counter = 12;
                ScrollImage();
            };

            //other
            other = new Button();
            cont_profile.AddChild(other);
            other.SetStyleAuto(null);
            other.SetPosition((int)(dim.XScreenRatio * 1440), (int)(dim.YScreenRatio * 50));
            other.SetSize((int)(dim.XScreenRatio * 300), (int)(dim.YScreenRatio * 100));
            other.Texture = generic_bts;
            other.ImageRect = new IntRect(0, 0, 260, 110);
            Text buttonother = new Text();
            other.AddChild(buttonother);
            buttonother.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonother.SetPosition(0, 0);
            buttonother.SetFont(font, dim.XScreenRatio * 30);
            buttonother.Value = "OTHER";
            other.Pressed += args => {
                 counter = 24;
                 ScrollImage();
             };

            // Buttons container (root element)
            cont_img = root.CreateSprite();
            cont_img.Texture = cont_base;
            cont_img.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 800));
            cont_img.SetPosition((int)(dim.XScreenRatio * 10), (int)(dim.YScreenRatio * 250));
            cont_img.ImageRect = new IntRect(0, 0, 56, 56);

            p_prev_img = new Button();
            cont_profile.AddChild(p_prev_img);
            p_prev_img.SetStyleAuto(null);
            p_prev_img.Opacity = 0.25f;
            p_prev_img.SetPosition((int)(dim.XScreenRatio * -400), (int)(dim.YScreenRatio * 300));
            p_prev_img.SetSize((int)(dim.XScreenRatio * 450), (int)(dim.YScreenRatio * 450));
            p_prev_img.Texture = profiles;
            p_prev_img.ImageRect = new IntRect(0, 0, 250, 250);

            prev_img = new Button();
            cont_profile.AddChild(prev_img);
            prev_img.SetStyleAuto(null);
            prev_img.Opacity = 0.5f;
            prev_img.SetPosition((int)(dim.XScreenRatio * 100), (int)(dim.YScreenRatio * 250));
            prev_img.SetSize((int)(dim.XScreenRatio * 500), (int)(dim.YScreenRatio * 500));
            prev_img.Texture = profiles;
            prev_img.ImageRect = new IntRect(0, 0, 250, 250);
            prev_img.Pressed += args => {
                if(counter <= 0) {
                    counter = 44;
                    //prev_img.Enabled = false;
                }
                else {
                    prev_img.Enabled = true;
                    counter = counter - 1;
                }
                ScrollImage();
            };

            sel_img = new Button();
            cont_profile.AddChild(sel_img);
            sel_img.SetStyleAuto(null);
            sel_img.SetPosition((int)(dim.XScreenRatio * 650), (int)(dim.YScreenRatio * 200));
            sel_img.SetSize((int)(dim.XScreenRatio * 550), (int)(dim.YScreenRatio * 550));
            sel_img.Texture = profiles;
            sel_img.ImageRect = new IntRect(250, 0, 500, 250);

            next_img = new Button();
            cont_profile.AddChild(next_img);
            next_img.Opacity = 0.5f;
            next_img.SetStyleAuto(null);
            next_img.SetPosition((int)(dim.XScreenRatio * 1250), (int)(dim.YScreenRatio * 250));
            next_img.SetSize((int)(dim.XScreenRatio * 500), (int)(dim.YScreenRatio * 500));
            next_img.Texture = profiles;
            next_img.ImageRect = new IntRect(500, 0, 750, 250);
            next_img.Pressed += args => {
                if(counter == 44) {
                    //next_img.Enabled = false;
                    counter = 0;
                }
                else {
                    next_img.Enabled = true;
                    counter = counter + 1;
                }
                ScrollImage();
            };

            n_next_img = new Button();
            cont_profile.AddChild(n_next_img);
            n_next_img.SetStyleAuto(null);
            n_next_img.Opacity = 0.25f;
            n_next_img.SetPosition((int)(dim.XScreenRatio * 1800), (int)(dim.YScreenRatio * 300));
            n_next_img.SetSize((int)(dim.XScreenRatio * 450), (int)(dim.YScreenRatio * 450));
            n_next_img.Texture = profiles;
            n_next_img.ImageRect = new IntRect(0, 0, 250, 250);
        }

        void ScrollImage() {
            if (counter < 0) {
                counter = 0;
            }
            var character = JsonReaderCharacter.GetSingleCharacter(counter);

            int type = character.Type;
            int left = character.ImagePosition.Left;
            int top = character.ImagePosition.Top;
            int right = character.ImagePosition.Right;
            int bottom = character.ImagePosition.Bottom;
            if (counter == 0) {
                p_prev_img.ImageRect = new IntRect(750, 1250, 1000, 1500);
                prev_img.ImageRect = new IntRect(1000, 1250, 1250, 1500);
                sel_img.ImageRect = new IntRect(0, 0, 250, 250);
                next_img.ImageRect = new IntRect(250, 0, 500, 250);
                male.ImageRect = new IntRect(260, 0, 520, 110);
                female.ImageRect = new IntRect(0, 0, 260, 110);
                other.ImageRect = new IntRect(0, 0, 260, 110);
                _nameText = "Rudy Reckless";
                lineEdit.Text = _nameText;
            }
            else if (counter == 1) {
                p_prev_img.ImageRect = new IntRect(1250, 1250, 1500, 1500);
                character = JsonReaderCharacter.GetSingleCharacter(0);
                int left_p = character.ImagePosition.Left;
                int top_p = character.ImagePosition.Top;
                int right_p = character.ImagePosition.Right;
                int bottom_p = character.ImagePosition.Bottom;
                prev_img.ImageRect = new IntRect(0, 0, 250, 250); 
                prev_img.Enabled = true;
                sel_img.ImageRect = new IntRect(250, 0, 500, 250);
                next_img.ImageRect = new IntRect(500, 0, 750, 250);
                male.ImageRect = new IntRect(260, 0, 520, 110);
                female.ImageRect = new IntRect(0, 0, 260, 110);
                other.ImageRect = new IntRect(0, 0, 260, 110);
                _nameText = "Rudy Reckless";
                lineEdit.Text = _nameText;
            }
            else {
                sel_img.ImageRect = new IntRect(left, top, right, bottom);
                next_img.Enabled = true;
                sel_img.Enabled = true;
                prev_img.Enabled = true;
                switch(type) {
                    case 0:
                        male.ImageRect = new IntRect(260, 0, 520, 110);
                        female.ImageRect = new IntRect(0, 0, 260, 110);
                        other.ImageRect = new IntRect(0, 0, 260, 110);
                        _nameText = "Rudy Reckless";
                        lineEdit.Text = _nameText;
                        break;
                    case 1:
                        male.ImageRect = new IntRect(0, 0, 260, 110);
                        female.ImageRect = new IntRect(260, 0, 520, 110);
                        other.ImageRect = new IntRect(0, 0, 260, 110);
                        _nameText = "Lauren Leadfoot";
                        lineEdit.Text = _nameText;
                        break;
                    case 2:
                        male.ImageRect = new IntRect(0, 0, 260, 110);
                        female.ImageRect = new IntRect(0, 0, 260, 110);
                        other.ImageRect = new IntRect(260, 0, 520, 110);
                        _nameText = "Alex Axxel";
                        lineEdit.Text = _nameText;
                        break;
                }
                int p_prev;
                int prev;
                int next;
                int n_next;
                if (counter == 43) {
                    p_prev = counter - 2;
                    prev = counter - 1;
                    next = 44;
                    n_next = 0;
                }
                else if (counter == 44) {
                    p_prev = counter - 2;
                    prev = counter - 1;
                    next = 0;
                    n_next = 1;
                }
                else {
                    p_prev = counter - 2;
                    prev = counter - 1;
                    next = counter + 1;
                    n_next =counter +2;
                }
                
                character = JsonReaderCharacter.GetSingleCharacter(p_prev);
                int left_pp = character.ImagePosition.Left;
                int top_pp = character.ImagePosition.Top;
                int right_pp = character.ImagePosition.Right;
                int bottom_pp = character.ImagePosition.Bottom;
                p_prev_img.ImageRect = new IntRect(left_pp, top_pp, right_pp, bottom_pp);

                character = JsonReaderCharacter.GetSingleCharacter(prev);
                int left_p = character.ImagePosition.Left;
                int top_p = character.ImagePosition.Top;
                int right_p = character.ImagePosition.Right;
                int bottom_p = character.ImagePosition.Bottom;
                prev_img.ImageRect = new IntRect(left_p, top_p, right_p, bottom_p);

                character = JsonReaderCharacter.GetSingleCharacter(next);
                int left_n = character.ImagePosition.Left;
                int top_n = character.ImagePosition.Top;
                int right_n = character.ImagePosition.Right;
                int bottom_n = character.ImagePosition.Bottom;
                next_img.ImageRect = new IntRect(left_n, top_n, right_n, bottom_n);

                character = JsonReaderCharacter.GetSingleCharacter(n_next);
                int left_nn = character.ImagePosition.Left;
                int top_nn = character.ImagePosition.Top;
                int right_nn = character.ImagePosition.Right;
                int bottom_nn = character.ImagePosition.Bottom;
                n_next_img.ImageRect = new IntRect(left_nn, top_nn, right_nn, bottom_nn);
            }
        }

        void SelectedImage() {
            if(CharacterManager.Instance.User != null) {
                var user = CharacterManager.Instance.User;
                user.PortraitId = counter;
                CharacterManager.Instance.User = user;
            }
        }
    }
}
