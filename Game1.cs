using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;
using Myra;
using System.Collections.Generic;
using System.IO;
using System;

namespace KNTGame;
public class Game1 :Game
{
  /*
    https://github.com/FontStashSharp/FontStashSharp
    MonoGame.Primitives2D
    https://github.com/0xLaileb/IniFile
    https://code.tutsplus.com/quick-tip-use-quadtrees-to-detect-likely-collisions-in-2d-space--gamedev-374t
  https://gamefromscratch.com/monogame-tutorial-handling-keyboard-mouse-and-gamepad-input/
  https://github.com/mdqinc/SDL_PlayerControllerDB
  https://community.monogame.net/t/nintendo-nes-joycon-does-not-recognize-select/19440/12
   */

  ButtonState CurLeftMouseButtonState;
  ButtonState LastLeftMouseButtonState;
  ButtonState CurRightMouseButtonState;
  ButtonState LastRightMouseButtonState;

  Byte NinpouPower;

  Button MainMenuStartButton;
  Button MainMenuCreditsButton;
  Button MainMenuOptionsButton;
  Button MainMenuManualButton;
  Button MainMenuExitButton;
  Label MainMenuVersionLabel;
  Label MainMenuTitleLabel;

  Button GameOverScreenExitButton;
  Button GameOverScreenRestartButton;
  Button GameOverScreenMainMenuButton;
  Label GameOverScreenTitleLabel;

  Button CreditsScreenMainMenuButton;
  Label CreditsScreenTextLabel;

  Label ManualScreenTextLabel;
  Button ManualScreenMainMenuButton;

  Label OptionsMenuLabel1;
  CheckButton chbAutoShoot;

  Label OptionsMenuLabel21;
  HorizontalSlider hsEnemySpawnInterval;
  Label OptionsMenuLabel22;

  Label OptionsMenuLabel31;
  HorizontalSlider hsEnemySpawnNumberMin;
  Label OptionsMenuLabel32;

  Label OptionsMenuLabel41;
  HorizontalSlider hsEnemySpawnNumberMax;
  Label OptionsMenuLabel42;

  Label OptionsMenuLabel51;
  HorizontalSlider hsPollyInvulnerabilityPeriod;
  Label OptionsMenuLabel52;

  Label OptionsMenuLabel61;
  HorizontalSlider hsNumberOfKilledEnemiesToSpawnHealth;
  Label OptionsMenuLabel62;

  Label OptionsMenuLabel71;
  HorizontalSlider hsMaxPlayerHealth;
  Label OptionsMenuLabel72;

  Label OptionsMenuLabel81;
  HorizontalSlider hsHealthVisibilityTime;
  Label OptionsMenuLabel82;
  CheckButton chbDontHideHealth;

  Label OptionsMenuLabel91;
  CheckButton chbKeyboardAndMouseControl;
  Label OptionsMenuLabel92;
  CheckButton chbGamepadControl;
  Label OptionsMenuLabel93;

  Label LevelInfoLabel1;
  Label LevelInfoLabel2;
  Label LevelInfoLabel3;

  //HorizontalSlider hsMusicVolume;

  Button OptionsMenuResetButton;
  Button OptionsMenuMainMenuButton;
  Dictionary<String, Texture2D> dictPollyDirectionSprites;
  Dictionary<String, Texture2D> dictCrowDirectionSprites;
  Dictionary<String, Texture2D> dictElectroDirectionSprites;

  double LastPlayerBulletShootTime = -0.5;
  double LastLeftDpadCheckTime = -0.5;

  public static float SpriteResizeKoof = 1.5f;

  private Desktop _desktop;
  Texture2D HpHeartTexture;
  enum GameState { MainMenu, Credits, Options, Playing, Manual, GameOver }
  GameState CurGameState;
  public enum PlayerControlDevice { KeyboardandMouse, Gamepad }
  public enum EnemyType { Crow, Electro };

  double currentTime1 = 0;
  double currentTime2 = 0;
  double currentTime3 = 0;
  double currentTime4 = 0;

  public Int32 UpdateCount = 0;
  public Int32 UpdateCount2 = 0;

  Int32 NumberOfKilledEnemiesToSpawnHealth = 15;

  float EnemySpawnTimerInterval = 3f; // First time - after 3s on from start
  float ElectroFireInterval = 6f; // First time - after 3s on from start
  float PollyInvulnerabilityInterval = 3f;

  Int16 EnemiesKilled;
  Int16 EnemiesKilledSinceHPTakenOrHidden;

  Grid grid;
  MouseState MouseState;

  PlayerSprite ObjPlayer;
  HpHeartSprite ObjHpHeart;
  Point MousePosition;
  ObjSettingsClass ObjSettings;

  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private FontSystem _fontSystem;

  List<EnemySprite> ListOfEnemies;
  List<EnemySprite> ListOfKilledEnemies;
  List<PlayerBulletSprite> ListOfLostPlayerBullets;
  List<PlayerBulletSprite> ListOfPlayerBullets;
  List<EnemyBulletSprite> ListOfEnemyBullets;
  List<EnemyBulletSprite> ListOfLostEnemyBullets;

  static float cos45 = (float)(Math.Sqrt(2) / 2);

  List<float> ListOfPlayerNinpoudx = new List<float> { 1, cos45, 0, -cos45, -1, -cos45, 0, cos45 };
  List<float> ListOfPlayerNinpoudy = new List<float> { 0, -cos45, -1, -cos45, 0, cos45, 1, cos45 };

  public static Int32 screen_height_full = 600;
  public static Int32 screen_height_playarea = 600 - 25;
  public static Int32 screen_width = 800;

  Rectangle GameArea;
  Rectangle LevelInfoRectangle;

  QuadtreeEnemies quadenemies;
  IniFile iniFile;

  float PlayerBulletInterval = 0.2f;
  Int32 ActiveControlID = 10;

  SolidBrush ButtonDefaultColor = new SolidBrush(Color.Green);
  SolidBrush ButtonHoveredColor = new SolidBrush(Color.LightGreen);

  Color LabelActiveColor =  Color.LightGreen;

  private static string GetSpriteFrameNumber1(Int32 TickCount_)
  {
    Int16 mod_ = Convert.ToInt16((TickCount_ % 15) + 1);

    if ((mod_ >= 1) && (mod_ <= 5))
    {
      return "1";
    }
    else if ((mod_ >= 6) && (mod_ <= 10))
    {
      return "2";
    }
    else if ((mod_ >= 11) && (mod_ <= 15))
    {
      return "3";
    }
    else
    {
      return "2";
    }
  }
  public string GetSpriteFrameNumber2(Int32 TickCount_)
  {
    Int16 mod_ = Convert.ToInt16((TickCount_ % 20) + 1);

    if ((mod_ >= 1) && (mod_ <= 5))
    {
      return "1";
    }
    else if ((mod_ >= 6) && (mod_ <= 10))
    {
      return "2";
    }
    else if ((mod_ >= 11) && (mod_ <= 15))
    {
      return "3";
    }
    else if ((mod_ >= 16) && (mod_ <= 20))
    {
      return "4";
    }
    else
    {
      return "2";
    }
  }

  public string GetKeyForPollyDirectionDictionary()
  {
    string sTmp = ObjPlayer.direction;
    Int32 vTickCount;

    if (!ObjPlayer.IsMoving)
    {
      sTmp = sTmp + "0";
    }
    else
    {
      vTickCount = (UpdateCount - ObjPlayer.MovingStatusChangeTime);
      Int16 mod_ = Convert.ToInt16((vTickCount % 20) + 1);

      if ((mod_ >= 1) && (mod_ <= 5))
      {
        sTmp = sTmp + "1";
      }
      else if ((mod_ >= 6) && (mod_ <= 10))
      {
        sTmp = sTmp + "2";
      }
      else if ((mod_ >= 11) && (mod_ <= 15))
      {
        sTmp = sTmp + "3";
      }
      else if ((mod_ >= 16) && (mod_ <= 20))
      {
        sTmp = sTmp + "4";
      }
      else
      {
        sTmp = sTmp + "2";
      }
    }

    return sTmp;
  }

  public static void Getdxdy(float x1, float y1, float x2, float y2, out float dx, out float dy)
  {
    double Atan2R = Math.Atan2(y2 - y1, x2 - x1);

    dx = Math.Abs((float)Math.Cos(Atan2R));
    dy = Math.Abs((float)Math.Sin(Atan2R));

    if (y2 < y1) { dy = dy * -1; }
    if (x2 < x1) { dx = dx * -1; }
  }
  public void PrepareGame()
  {
    ActiveControlID = 10;
    currentTime1 = 0;
    currentTime2 = 0;
    currentTime3 = 0;
    currentTime4 = 0;
    EnemiesKilledSinceHPTakenOrHidden = 0;
    EnemiesKilled = 0;
    LastPlayerBulletShootTime = -0.5;
    LastLeftDpadCheckTime = -0.5;
    ListOfEnemies.Clear();
    ListOfEnemyBullets.Clear();
    ListOfKilledEnemies.Clear();
    ListOfLostEnemyBullets.Clear();
    ListOfLostPlayerBullets.Clear();
    ListOfPlayerBullets.Clear();
    NinpouPower = 0;
    ObjPlayer.direction = "Right";
    ObjPlayer.Health = ObjSettings.MaxPlayerHealth;
    ObjPlayer.IsInvulnerable = false;
    ObjPlayer.IsMoving = false;
    ObjPlayer.MovingStatusChangeTime = 0;
    ObjPlayer.InvulnerabilityStatusChangeTime = 0;
    ObjHpHeart.Visible = false;
    ObjHpHeart.AppearanceTime = 0;
    quadenemies.clear();
    UpdateCount = 0;
    UpdateCount2 = 0;
  }

  public void SpawnEnemies()
  {
    Texture2D Texture_;

    for (int i = 1;
    i <= new Random().Next(ObjSettings.EnemySpawnNumberMin, ObjSettings.EnemySpawnNumberMax + 1);
    i = i + 1)

    {
      Random rnd = new Random();

      int x0 = rnd.Next(0, screen_width);
      int y0 = rnd.Next(0, screen_height_full);
      float x = 0.0f;
      float y = 0.0f;

      int direction = rnd.Next(1, 5);

      switch (direction)
      {
        case 1: 
          x = x0;
          y = -5;
          break;

        case 2: 
          x = screen_width + 5;
          y = y0;
          break;

        case 3: 
          x = x0;
          y = screen_height_full + 5;
          break;

        case 4: 
          x = -5;
          y = y0;
          break;
      }

      int rnd_enemytype = rnd.Next(1, 3);

      if (rnd_enemytype == 1)
      {
        Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Crow_Right_1");

        EnemySpriteCrow sprite = new EnemySpriteCrow(texture_: Texture_
        , position_: new Vector2(x, y)
        , direction_: "Right"
        , type_: EnemyType.Crow
        , LastDirectionChangeTime_: UpdateCount);

        ListOfEnemies.Add(sprite);
      }
      else
      {
        Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Electro_2");

        EnemySpriteElectro sprite =
        new EnemySpriteElectro(texture_: Texture_
        , position_: new Vector2(x, y)
        , direction_: ""
        , type_: EnemyType.Electro
        , LastFireStateTriggerTime_: UpdateCount);

        sprite.IsAttacking = false;
        sprite.LastFireStateTriggerTime = UpdateCount;
        ListOfEnemies.Add(sprite);
      }
    }
  }

  public void ElectroEnemyFireLightning()
  {
    Texture2D Texture_;
    float DeltaX, DeltaY;

    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Electro_Weapon");

    foreach (EnemySprite EnemySprite_ in ListOfEnemies)
    {
      if (EnemySprite_.Type != EnemyType.Electro)
      {
        continue;
      }

      if (!(EnemySprite_ as EnemySpriteElectro).IsInsideRoom())
      {
        continue;
      }

      Getdxdy(x1: EnemySprite_.position.X, y1: EnemySprite_.position.Y,
      x2: ObjPlayer.position.X, y2: ObjPlayer.position.Y,
      dx: out DeltaX, dy: out DeltaY);

      /*
      Atan2R = Math.Atan2(ObjPlayer.position.Y - EnemySprite_.position.Y,
      ObjPlayer.position.X - EnemySprite_.position.X);

      abs_dx = Math.Abs((float)Math.Cos(Atan2R));
      abs_dy = Math.Abs((float)Math.Sin(Atan2R));

      if (ObjPlayer.position.X > EnemySprite_.position.X)
      {
        DeltaX = abs_dx;
      }
      else
      {
        DeltaX = -1 * abs_dx;
      }

      if (ObjPlayer.position.Y > EnemySprite_.position.Y)
      {
        DeltaY = abs_dy;
      }
      else
      {
        DeltaY = -1 * abs_dy;
      }
*/

      EnemyBulletSprite EnemyBulletSprite_ = new EnemyBulletSprite(texture_: Texture_

      , position_: new Vector2(
      EnemySprite_.position.X + (float)0.5 * (EnemySprite_.texture.Height - Texture_.Height)
      , EnemySprite_.position.Y + (float)0.5 * (EnemySprite_.texture.Width - Texture_.Width)
      )
      , deltax_: (float)DeltaX
      , deltay_: (float)DeltaY);

      (EnemySprite_ as EnemySpriteElectro).IsAttacking = true;
      (EnemySprite_ as EnemySpriteElectro).LastFireStateTriggerTime = UpdateCount;
      ListOfEnemyBullets.Add(EnemyBulletSprite_);
    }
  }

  public void SetRectMoveLimits(EnemySprite cur_sprite, EnemySprite ext_sprite)
  {
    cur_sprite.CanGoRight = !(ext_sprite.position.Y > cur_sprite.position.Y);
    cur_sprite.CanGoLeft = !(ext_sprite.position.Y < cur_sprite.position.Y);
    cur_sprite.CanGoDown = !(ext_sprite.position.X > cur_sprite.position.X);
    cur_sprite.CanGoUp = !(ext_sprite.position.X < cur_sprite.position.X);

    /*
    float dy = ext_sprite.position.Y - cur_sprite.position.Y;
    float dx = ext_sprite.position.X - cur_sprite.position.X;

    Double Atan2R = Math.Atan2(dy, dx);
    Double Atan2D = Atan2R / Math.PI * 180;

    if (Atan2D < 0) { Atan2D = Atan2D + 360; }
    */
  }

  public void SpawnHealthHeart()
  {
    if (ObjHpHeart.Visible || (EnemiesKilledSinceHPTakenOrHidden < NumberOfKilledEnemiesToSpawnHealth))
    {
      return;
    }

    Random rnd = new Random();

    int x = rnd.Next(5, screen_width - HpHeartTexture.Width);
    int y = rnd.Next(30, screen_height_playarea - HpHeartTexture.Height);

    ObjHpHeart.position.X = x;
    ObjHpHeart.position.Y = y;
    ObjHpHeart.Visible = true;
  }

  public void SaveGameSettings(out bool CheckSuccessful)
  {
    if (hsEnemySpawnNumberMin.Value > hsEnemySpawnNumberMax.Value)
    {
      var messageBox = Dialog.CreateMessageBox("Error", "Enemy spawn mumber Min > Enemy spawn number Max");
      messageBox.ShowModal(_desktop);
      CheckSuccessful = false;
      return;
    }

    if (chbGamepadControl.IsChecked == chbKeyboardAndMouseControl.IsChecked)
    {
      var messageBox = Dialog.CreateMessageBox("Error", "Check only one control - keyboard/mouse or gamepad.");
      messageBox.ShowModal(_desktop);
      CheckSuccessful = false;
      return;
    }

    ObjSettings.AutoShoot = chbAutoShoot.IsChecked;
    ObjSettings.EnemySpawnInterval = Convert.ToByte(hsEnemySpawnInterval.Value);
    ObjSettings.EnemySpawnNumberMin = Convert.ToByte(hsEnemySpawnNumberMin.Value);
    ObjSettings.EnemySpawnNumberMax = Convert.ToByte(hsEnemySpawnNumberMax.Value);
    ObjSettings.PollyInvulnerabilityPeriod = Convert.ToByte(hsPollyInvulnerabilityPeriod.Value);
    ObjSettings.NumberOfKilledEnemiesToSpawnHealth = Convert.ToByte(hsNumberOfKilledEnemiesToSpawnHealth.Value);
    ObjSettings.MaxPlayerHealth = Convert.ToByte(hsMaxPlayerHealth.Value);
    ObjSettings.HealthVisibilityTime = Convert.ToByte(hsHealthVisibilityTime.Value);
    ObjSettings.DontHideHealth = chbDontHideHealth.IsChecked;

    if (chbKeyboardAndMouseControl.IsChecked)
    {
      ObjSettings.PlayerControl = PlayerControlDevice.KeyboardandMouse;
    }
    else
    {
      ObjSettings.PlayerControl = PlayerControlDevice.Gamepad;
    }

    EnemySpawnTimerInterval = ObjSettings.EnemySpawnInterval;
    PollyInvulnerabilityInterval = ObjSettings.PollyInvulnerabilityPeriod;
    NumberOfKilledEnemiesToSpawnHealth = ObjSettings.NumberOfKilledEnemiesToSpawnHealth;

    iniFile.Write("AutoShoot", ObjSettings.AutoShoot.ToString(), "Main");
    iniFile.Write("EnemySpawnInterval", ObjSettings.EnemySpawnInterval.ToString(), "Main");
    iniFile.Write("EnemySpawnNumberMin", ObjSettings.EnemySpawnNumberMin.ToString(), "Main");
    iniFile.Write("EnemySpawnNumberMax", ObjSettings.EnemySpawnNumberMax.ToString(), "Main");
    iniFile.Write("PollyInvulnerabilityPeriod", ObjSettings.PollyInvulnerabilityPeriod.ToString(), "Main");
    iniFile.Write("NumberOfKilledEnemiesToSpawnHealth", ObjSettings.NumberOfKilledEnemiesToSpawnHealth.ToString(), "Main");
    iniFile.Write("MaxPlayerHealth", ObjSettings.MaxPlayerHealth.ToString(), "Main");
    iniFile.Write("HealthVisibilityTime", ObjSettings.HealthVisibilityTime.ToString(), "Main");
    // iniFile.Write("MusicVolume", ObjSettings.MusicVolume.ToString(), "Main");
    iniFile.Write("PlayerControl", ObjSettings.PlayerControl.ToString(), "Main");
    iniFile.Write("DontHideHealth", ObjSettings.DontHideHealth.ToString(), "Main");
    CheckSuccessful = true;
  }

  public void LoadGameSettings()
  {
    chbAutoShoot.IsChecked = ObjSettings.AutoShoot;
    hsEnemySpawnInterval.Value = ObjSettings.EnemySpawnInterval;
    hsEnemySpawnNumberMin.Value = ObjSettings.EnemySpawnNumberMin;
    hsEnemySpawnNumberMax.Value = ObjSettings.EnemySpawnNumberMax;
    hsPollyInvulnerabilityPeriod.Value = ObjSettings.PollyInvulnerabilityPeriod;
    hsNumberOfKilledEnemiesToSpawnHealth.Value = ObjSettings.NumberOfKilledEnemiesToSpawnHealth;
    hsMaxPlayerHealth.Value = ObjSettings.MaxPlayerHealth;
    //  hsMusicVolume.Value = ObjSettings.MusicVolume;
    chbGamepadControl.IsChecked = (ObjSettings.PlayerControl == PlayerControlDevice.Gamepad);
    chbKeyboardAndMouseControl.IsChecked = (ObjSettings.PlayerControl == PlayerControlDevice.KeyboardandMouse);
  }

  public void CreateGamePlayControls()
  {
    LevelInfoLabel1 = new Label
    {
      Width = 100, Text = "hjhj",
      Left = 0, Top = 0
   , TextColor = Color.Green
     , Font = _fontSystem.GetFont(23), Wrap = true
    };

    LevelInfoLabel2 = new Label
    {
      Width = (screen_width - 200) / 2, Text = "",
      Left = 100, Top = 0
, TextColor = Color.Green
, Font = _fontSystem.GetFont(23), Wrap = true
    };

    LevelInfoLabel3 = new Label
    {
      Width = 100, Text = "",
      Left = (screen_width - 100), Top = 0
, TextColor = Color.Green
, Font = _fontSystem.GetFont(23), Wrap = true
    };

    grid.Widgets.Add(LevelInfoLabel1);
    grid.Widgets.Add(LevelInfoLabel2);
    grid.Widgets.Add(LevelInfoLabel3);
  }

  public void HideAndDisposeGamePlayControls()
  {
    LevelInfoLabel1.Visible = false;
    LevelInfoLabel2.Visible = false;
    LevelInfoLabel3.Visible = false;
    
    LevelInfoLabel1=null;
    LevelInfoLabel2 = null;
    LevelInfoLabel3 = null;
  }

  public void CreateManualScreenControls()
  {
    string sText = "";

    if (ObjSettings.PlayerControl == PlayerControlDevice.KeyboardandMouse)
    {
      sText = "How to play";
      sText = sText + (char)10 + (char)10;

      sText = sText + "Use W, A, S and D key to control Polly.";
      sText = sText + (char)10 + (char)10;

      sText = sText + "Click left mouse button to fire a weapon towards mouse location.";
      sText = sText + (char)10 + (char)10;

      sText = sText + "Each killed emeny gives 10 Ninpou power.";
      sText = sText + (char)10;

      sText = sText + "When Ninpou reaches 100 unleash Nyanki Ninpou by pressing right mouse button";
      sText = sText + (char)10;
      sText = sText + "Ninpou hearts one-shots enemies on their way.";
      sText = sText + (char)10 + (char)10;

      sText = sText + "Kill " + ObjSettings.NumberOfKilledEnemiesToSpawnHealth + " enemies to spawn a HP-restore heart";
      sText = sText + (char)10;

      sText = sText + "Ememies have 3 HP, player has " + ObjSettings.MaxPlayerHealth + " HP.";
      sText = sText + (char)10 + (char)10;

      sText = sText + "When player's health reaches 0 game is over";
    }
    else

    {
      sText = "How to play";
      sText = sText + (char)10 + (char)10;

      sText = sText + "Use D-pad to control Polly.";
      sText = sText + (char)10 + (char)10;

      sText = sText + "Lean right thumbstick to fire a weapon";
      sText = sText + (char)10 + (char)10;

      sText = sText + "Each killed emeny gives 10 Ninpou power.";
      sText = sText + (char)10;

      sText = sText + "When Ninpou reaches 100 unleash Nyanki Ninpou by pressing X button on controller";
      sText = sText + (char)10;
      sText = sText + "Ninpou hearts one-shots enemies on their way.";
      sText = sText + (char)10 + (char)10;

      sText = sText + "Kill " + ObjSettings.NumberOfKilledEnemiesToSpawnHealth + " enemies to spawn a HP-restore heart";
      sText = sText + (char)10;

      sText = sText + "Ememies have 3 HP, player has " + ObjSettings.MaxPlayerHealth + " HP.";
      sText = sText + (char)10 + (char)10;

      sText = sText + "When player's health reaches 0 game is over";
    }

    ManualScreenTextLabel = new Label
    {
      Wrap = true, Text = sText, Font = _fontSystem.GetFont(25), Left = 5, Top = 5, Width = screen_width - 5,
      Height = screen_height_playarea - 15, TextColor = Color.Black
    };

    ManualScreenMainMenuButton = new Button
    {
      Left = 310, Top = 400,
      Width = screen_width - (2 * 310),
      Height = 40,
      BorderThickness = new Thickness(1),
      Background = ButtonDefaultColor,
      OverBackground = ButtonHoveredColor,
      PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Main Menu", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    ManualScreenMainMenuButton.Click += (s, a) =>
    {
      HideAndDisposeManualScreenControls();
      ActiveControlID = 0;
      CurGameState = GameState.MainMenu;
    };

    grid.Widgets.Add(ManualScreenTextLabel);
    grid.Widgets.Add(ManualScreenMainMenuButton);
  } //CreateManualScreenControls

  public void HideAndDisposeManualScreenControls()
  {
    ManualScreenMainMenuButton.Visible = false;
    ManualScreenTextLabel.Visible = false;

    ManualScreenMainMenuButton = null;
    ManualScreenTextLabel = null;
  }

  public void CreateOptionsMenuControls()
  {
    #region Options menu

    OptionsMenuLabel1 = new Label
    {
      Width = 250, Text = "Autoshoot", Font = _fontSystem.GetFont(23),
      Left = 5, Top = 50
     ,TextColor = Color.Black
    };

    chbAutoShoot = new CheckButton
    {
      Content = new Label
      {
        Font = _fontSystem.GetFont(25),
        TextColor = Color.Black
      },
      Width = 50, Left = 260, Top = 50
    };

    OptionsMenuLabel21 = new Label
    {
      Width = 250, Text = "Enemy spawn interval (seconds)",
      Left = 5, Top = 90
    , TextColor = Color.Black
      , Font = _fontSystem.GetFont(23), Wrap = true
    };

    hsEnemySpawnInterval = new HorizontalSlider
    {
      Width = 120, Left = 260, Top = 90,
      Minimum = 5, Maximum = 10
     , Value = ObjSettings.EnemySpawnInterval
    };

    hsEnemySpawnInterval.ValueChanged += (s, a) =>
    {
      OptionsMenuLabel22.Text = Math.Round(hsEnemySpawnInterval.Value).ToString();
    };

    OptionsMenuLabel22 = new Label
    {
      Width = 30, Text = ObjSettings.EnemySpawnInterval.ToString(),
      Left = 390, Top = 90, TextColor = Color.Black,
      Font = _fontSystem.GetFont(23)
     , Background = new SolidBrush(Color.White)
    };

    OptionsMenuLabel31 = new Label
    {
      Width = 250, Text = "Number of enemies to spawn per timer (Min)",
      Left = 5, Top = 140
     ,TextColor = Color.Black
      , Wrap = true,
      Font = _fontSystem.GetFont(23)
    };

    hsEnemySpawnNumberMin = new HorizontalSlider
    {
      Width = 120, Left = 260, Top = 140,
      Minimum = 3, Maximum = 15, Value = ObjSettings.EnemySpawnNumberMin
    };

    hsEnemySpawnNumberMin.ValueChanged += (s, a) =>
    {
      OptionsMenuLabel32.Text = Math.Round(hsEnemySpawnNumberMin.Value).ToString();
    };

    OptionsMenuLabel32 = new Label
    {
      Width = 30, Text = ObjSettings.EnemySpawnNumberMin.ToString(),
      Left = 390, Top = 140, TextColor = Color.Black,
      Font = _fontSystem.GetFont(23)
  , Background = new SolidBrush(Color.White)
    };

    OptionsMenuLabel41 = new Label
    {
      Width = 250, Text = "Number of enemies to spawn per timer (Max)",
      Left = 5, Top = 205, TextColor = Color.Black, Wrap = true,
      Font = _fontSystem.GetFont(23)
    };

    hsEnemySpawnNumberMax = new HorizontalSlider
    {
      Width = 120, Left = 260, Top = 205,
      Minimum = 6, Maximum = 25,
      Value = ObjSettings.EnemySpawnNumberMax
    };

    hsEnemySpawnNumberMax.ValueChanged += (s, a) =>
    {
      OptionsMenuLabel42.Text = Math.Round(hsEnemySpawnNumberMax.Value).ToString();
    };

    OptionsMenuLabel42 = new Label
    {
      Width = 30, Text = ObjSettings.EnemySpawnNumberMax.ToString(),
      Left = 390, Top = 205, TextColor = Color.Black,
      Font = _fontSystem.GetFont(23)
  , Background = new SolidBrush(Color.White)
    };

    OptionsMenuLabel51 = new Label
    {
      Width = 250, Text = "Polly invulnerability period after hit (seconds)",
      Left = 5, Top = 260, TextColor = Color.Black, Wrap = true,
      Font = _fontSystem.GetFont(23)
    };

    hsPollyInvulnerabilityPeriod = new HorizontalSlider
    {
      Width = 120, Left = 260, Top = 260,
      Minimum = 1, Maximum = 3
     , Value = ObjSettings.PollyInvulnerabilityPeriod
    };

    hsPollyInvulnerabilityPeriod.ValueChanged += (s, a) =>
    {
      OptionsMenuLabel52.Text = Math.Round(hsPollyInvulnerabilityPeriod.Value).ToString();
    };

    OptionsMenuLabel52 = new Label
    {
      Width = 30, Text = ObjSettings.PollyInvulnerabilityPeriod.ToString(),
      Left = 390, Top = 260, TextColor = Color.Black,
      Font = _fontSystem.GetFont(23)
  , Background = new SolidBrush(Color.White)
    };

    OptionsMenuLabel61 = new Label
    {
      Width = 250, Text = "Number of killed enemies to spawn Health",
      Left = 5, Top = 320, TextColor = Color.Black, Wrap = true,
      Font = _fontSystem.GetFont(23)
    };

    hsNumberOfKilledEnemiesToSpawnHealth = new HorizontalSlider
    {
      Width = 120, Left = 260, Top = 320,
      Minimum = 10, Maximum = 25
     , Value = ObjSettings.NumberOfKilledEnemiesToSpawnHealth
    };

    hsNumberOfKilledEnemiesToSpawnHealth.ValueChanged += (s, a) =>
    {
      OptionsMenuLabel62.Text = Math.Round(hsNumberOfKilledEnemiesToSpawnHealth.Value).ToString();
    };

    OptionsMenuLabel62 = new Label
    {
      Width = 30, Text = ObjSettings.NumberOfKilledEnemiesToSpawnHealth.ToString(),
      Left = 390, Top = 320, TextColor = Color.Black,
      Font = _fontSystem.GetFont(23)
  , Background = new SolidBrush(Color.White)
    };

    OptionsMenuLabel71 = new Label
    {
      Width = 250, Text = "Max player health",
      Left = 5, Top = 375, TextColor = Color.Black, Wrap = true,
      Font = _fontSystem.GetFont(23)
    };

    hsMaxPlayerHealth = new HorizontalSlider
    {
      Width = 120, Left = 260, Top = 375,
      Minimum = 1, Maximum = 5
     , Value = ObjSettings.MaxPlayerHealth
    };

    hsMaxPlayerHealth.ValueChanged += (s, a) =>
    {
      OptionsMenuLabel72.Text = Math.Round(hsMaxPlayerHealth.Value).ToString();
    };

    OptionsMenuLabel72 = new Label
    {
      Width = 30, Text = ObjSettings.MaxPlayerHealth.ToString(),
      Left = 390, Top = 375, TextColor = Color.Black,
      Font = _fontSystem.GetFont(23)
  , Background = new SolidBrush(Color.White)
    };

    OptionsMenuLabel81 = new Label
    {
      Width = 250, Text = "Health visibility time (seconds)",
      Left = 5, Top = 420, TextColor = Color.Black, Wrap = true,
      Font = _fontSystem.GetFont(23)
    };

    hsHealthVisibilityTime = new HorizontalSlider
    {
      Width = 120, Left = 260, Top = 420,
      Minimum = 5, Maximum = 20
     , Value = ObjSettings.HealthVisibilityTime
    };

    hsHealthVisibilityTime.ValueChanged += (s, a) =>
    {
      OptionsMenuLabel82.Text = Math.Round(hsHealthVisibilityTime.Value).ToString();
    };

    OptionsMenuLabel82 = new Label
    {
      Width = 30, Text = ObjSettings.HealthVisibilityTime.ToString(),
      Left = 390, Top = 420, TextColor = Color.Black,
      Font = _fontSystem.GetFont(23)
, Background = new SolidBrush(Color.White)
    };

    chbDontHideHealth = new CheckButton
    {
      Content = new Label
      {
        Font = _fontSystem.GetFont(25),
        TextColor = Color.Black, Text = "Don't hide"
      },
      Width = 200, Left = 450, Top = 420,
      IsChecked = ObjSettings.DontHideHealth
    };

    OptionsMenuLabel91 = new Label
    {
      Width = 100, Text = "Player control",
      Left = 5, Top = 470, TextColor = Color.Black,
      Font = _fontSystem.GetFont(23)
    };

    chbKeyboardAndMouseControl = new CheckButton
    {
      Width = 200, Left = 150, Top = 475,
      IsChecked = (ObjSettings.PlayerControl == PlayerControlDevice.KeyboardandMouse)
    };

    OptionsMenuLabel92 =  new Label
    {
      Width = 100, Left = 170, Top = 470,
      Text = "Keyboard/Mouse"
     , TextColor = Color.Black,
      Font = _fontSystem.GetFont(23)
    };

    chbKeyboardAndMouseControl.PressedChanged += (s, a) =>
    {
      // chbGamepadControl.IsChecked = !(chbKeyboardAndMouseControl.IsChecked);
      if (chbKeyboardAndMouseControl.IsChecked) { chbGamepadControl.IsChecked = false; }

    };

    chbGamepadControl = new CheckButton
    {
      Width = 200, Left = 350, Top = 475,
      IsChecked = (ObjSettings.PlayerControl == PlayerControlDevice.Gamepad)
    };

    chbGamepadControl.PressedChanged += (s, a) =>
    {
      // chbKeyboardAndMouseControl.IsChecked = !(chbGamepadControl.IsChecked);
      if (chbGamepadControl.IsChecked) { chbKeyboardAndMouseControl.IsChecked = false; }

    };

    OptionsMenuLabel93 = new Label
    {
      Width = 100, Left = 370, Top = 470,
      Text = "Gamepad (xInput)"
 , TextColor = Color.Black
 , Font = _fontSystem.GetFont(23)
    };

    OptionsMenuResetButton = new Button
    {
      Left = 110, Top = screen_height_full - 40 - 5,
      Width = screen_width - (2 * 310),
      Height = 40,
      BorderThickness = new Thickness(1),
      Background = ButtonDefaultColor,
      OverBackground = ButtonHoveredColor,
      PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Reset", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    OptionsMenuResetButton.Click += (s, a) =>
    {
      var messageBox = Dialog.CreateMessageBox("Settings reset", "Reset settings?");
      messageBox.ShowModal(_desktop);

      messageBox.Closed += (s, a) =>
      {
        if (!messageBox.Result)
        {
          // Dialog was either closed or "Cancel" clicked
          return;
        }
        else
        {
          chbAutoShoot.IsChecked = true;
          hsEnemySpawnInterval.Value = 5;
          hsEnemySpawnNumberMin.Value = 3;
          hsEnemySpawnNumberMax.Value = 6;
          hsPollyInvulnerabilityPeriod.Value = 3;
          hsNumberOfKilledEnemiesToSpawnHealth.Value = 15;
          hsMaxPlayerHealth.Value = 3;
          //  hsMusicVolume.Value = 50;
        }
      };
    };

    OptionsMenuMainMenuButton = new Button
    {
      Left = 510, Top = screen_height_full - 40 - 5,
      Width = screen_width - (2 * 310),
      Height = 40,
      BorderThickness = new Thickness(1),
      Background = ButtonDefaultColor,
      OverBackground = ButtonHoveredColor,
      PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Main menu", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    OptionsMenuMainMenuButton.Click += (s, a) =>
    {
      bool bTmp;
      SaveGameSettings(out bTmp);

      if (bTmp == false) { return; }

      HideAndDisposeOptionsMenuControls();
      CurGameState = GameState.MainMenu;
    };
    #endregion options menu

    grid.Widgets.Add(OptionsMenuLabel1);
    grid.Widgets.Add(chbAutoShoot);

    grid.Widgets.Add(OptionsMenuLabel21);
    grid.Widgets.Add(hsEnemySpawnInterval);
    grid.Widgets.Add(OptionsMenuLabel22);

    grid.Widgets.Add(OptionsMenuLabel31);
    grid.Widgets.Add(hsEnemySpawnNumberMin);
    grid.Widgets.Add(OptionsMenuLabel32);

    grid.Widgets.Add(OptionsMenuLabel41);
    grid.Widgets.Add(hsEnemySpawnNumberMax);
    grid.Widgets.Add(OptionsMenuLabel42);

    grid.Widgets.Add(OptionsMenuLabel51);
    grid.Widgets.Add(hsPollyInvulnerabilityPeriod);
    grid.Widgets.Add(OptionsMenuLabel52);

    grid.Widgets.Add(OptionsMenuLabel61);
    grid.Widgets.Add(hsNumberOfKilledEnemiesToSpawnHealth);
    grid.Widgets.Add(OptionsMenuLabel62);

    grid.Widgets.Add(OptionsMenuLabel71);
    grid.Widgets.Add(hsMaxPlayerHealth);
    grid.Widgets.Add(OptionsMenuLabel72);

    grid.Widgets.Add(OptionsMenuLabel71);
    grid.Widgets.Add(hsMaxPlayerHealth);
    grid.Widgets.Add(OptionsMenuLabel72);

    grid.Widgets.Add(OptionsMenuLabel81);
    grid.Widgets.Add(hsHealthVisibilityTime);
    grid.Widgets.Add(OptionsMenuLabel82);
    grid.Widgets.Add(chbDontHideHealth);

    grid.Widgets.Add(OptionsMenuLabel91);
    grid.Widgets.Add(chbGamepadControl);
    grid.Widgets.Add(OptionsMenuLabel92);
    grid.Widgets.Add(chbKeyboardAndMouseControl);
    grid.Widgets.Add(OptionsMenuLabel93);

    grid.Widgets.Add(OptionsMenuResetButton);
    grid.Widgets.Add(OptionsMenuMainMenuButton);
  } //CreateOptionsMenuControls

  public void HideAndDisposeOptionsMenuControls()
  {
    OptionsMenuLabel1.Visible = false;
    chbAutoShoot.Visible = false;

    OptionsMenuLabel21.Visible = false;
    hsEnemySpawnInterval.Visible = false;
    OptionsMenuLabel22.Visible = false;

    OptionsMenuLabel31.Visible = false;
    hsEnemySpawnNumberMin.Visible = false;
    OptionsMenuLabel32.Visible = false;

    OptionsMenuLabel41.Visible = false;
    hsEnemySpawnNumberMax.Visible = false;
    OptionsMenuLabel42.Visible = false;

    OptionsMenuLabel51.Visible = false;
    hsPollyInvulnerabilityPeriod.Visible = false;
    OptionsMenuLabel52.Visible = false;

    OptionsMenuLabel61.Visible = false;
    hsNumberOfKilledEnemiesToSpawnHealth.Visible = false;
    OptionsMenuLabel62.Visible = false;

    OptionsMenuLabel71.Visible = false;
    hsMaxPlayerHealth.Visible = false;
    OptionsMenuLabel72.Visible = false;

    OptionsMenuLabel81.Visible = false;
    hsHealthVisibilityTime.Visible = false;
    OptionsMenuLabel82.Visible = false;
    chbDontHideHealth.Visible = false;

    OptionsMenuLabel91.Visible = false;
    OptionsMenuLabel92.Visible = false;
    OptionsMenuLabel93.Visible = false;

    chbGamepadControl.Visible = false;
    chbKeyboardAndMouseControl.Visible = false;

    OptionsMenuResetButton.Visible = false;
    OptionsMenuMainMenuButton.Visible = false;

    //  --------------------
    OptionsMenuLabel1 = null;
    chbAutoShoot = null;

    OptionsMenuLabel21 = null;
    hsEnemySpawnInterval = null;
    OptionsMenuLabel22 = null;

    OptionsMenuLabel31 = null;
    hsEnemySpawnNumberMin = null;
    OptionsMenuLabel32 = null;

    OptionsMenuLabel41 = null;
    hsEnemySpawnNumberMax = null;
    OptionsMenuLabel42 = null;

    OptionsMenuLabel51 = null;
    hsPollyInvulnerabilityPeriod = null;
    OptionsMenuLabel52 = null;

    OptionsMenuLabel61 = null;
    hsNumberOfKilledEnemiesToSpawnHealth = null;
    OptionsMenuLabel62 = null;

    OptionsMenuLabel71 = null;
    hsMaxPlayerHealth = null;
    OptionsMenuLabel72 = null;

    OptionsMenuLabel81 = null;
    hsHealthVisibilityTime = null;
    OptionsMenuLabel82 = null;

    OptionsMenuLabel91 = null;
    OptionsMenuLabel92 = null;
    OptionsMenuLabel93 = null;

    chbGamepadControl = null;
    chbKeyboardAndMouseControl = null;
    chbDontHideHealth = null;

    OptionsMenuResetButton = null;
    OptionsMenuMainMenuButton = null;
  } //HideAndDisposeOptionsMenuControls


  public void CreateMainMenuControls()
  {

    MainMenuStartButton = new Button
    {
      Left = 310, Top = 200
      , Width = screen_width - 2 * 310
      , Height = 40, BorderThickness = new Thickness(1)
      // , Background = (ActiveControlID == 10 ? ButtonHoveredColor : ButtonDefaultColor)
      // , OverBackground = (ActiveControlID == 10 ? ButtonHoveredColor : ButtonDefaultColor)
      , PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Start", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };
    MainMenuStartButton.Click += (s, a) =>
    {
      PrepareGame();
      CreateGamePlayControls();
      CurGameState = GameState.Playing;
    };
    MainMenuStartButton.MouseEntered += (s, a) =>
    {
      ActiveControlID = 10;
    };

    MainMenuOptionsButton = new Button
    {
      Left = 310, Top = 250,
      Width = screen_width - 2 * 310,
      Height = 40,
      BorderThickness = new Thickness(1)

     , Background = (ActiveControlID == 20 ? ButtonHoveredColor : ButtonDefaultColor)
     , OverBackground = ButtonHoveredColor
      , PressedBackground = ButtonDefaultColor

     , Content = new Label
     {
       HorizontalAlignment = HorizontalAlignment.Center,
       VerticalAlignment = VerticalAlignment.Center,
       Text = "Options", Font = _fontSystem.GetFont(30),
       TextColor = Color.Black
     }
    };

    MainMenuOptionsButton.Click += (s, a) =>
    {
      HideAndDisposeMainMenuControls();
      ActiveControlID = 10;
      CreateOptionsMenuControls();
      LoadGameSettings();
      CurGameState = GameState.Options;
    };

    MainMenuOptionsButton.MouseEntered += (s, a) =>
    {
      ActiveControlID = 20;
    };


    MainMenuManualButton = new Button
    {
      Left = 310, Top = 300,
      Width = screen_width - (2 * 310),
      Height = 40,
      BorderThickness = new Thickness(1)
     , Background = (ActiveControlID == 30 ? ButtonHoveredColor : ButtonDefaultColor)
     , OverBackground = ButtonHoveredColor
     , PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Manual", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    MainMenuManualButton.Click += (s, a) =>
    {
      MainMenuCreditsButton.Visible = false;
      MainMenuExitButton.Visible = false;
      MainMenuManualButton.Visible = false;
      MainMenuOptionsButton.Visible = false;
      MainMenuStartButton.Visible = false;
      MainMenuTitleLabel.Visible = false;
      ActiveControlID = 10;

      CreateManualScreenControls();
      CurGameState = GameState.Manual;
    };

    MainMenuManualButton.MouseEntered += (s, a) =>
    {
      ActiveControlID = 30;
    };

    MainMenuCreditsButton = new Button
    {
      Left = 310, Top = 350,
      Width = screen_width - (2 * 310),
      Height = 40,
      BorderThickness = new Thickness(1)
      , Background = (ActiveControlID == 40 ? ButtonHoveredColor : ButtonDefaultColor)
      , OverBackground = ButtonHoveredColor,
      PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Credits", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    MainMenuCreditsButton.Click += (s, a) => { CurGameState = GameState.Credits; };

    MainMenuCreditsButton.MouseEntered += (s, a) =>
    {
      ActiveControlID = 40;
    };

    MainMenuExitButton = new Button
    {
      Left = 310, Top = 400,
      Width = screen_width - (2 * 310),
      Height = 40,
      BorderThickness = new Thickness(1)
     , Background = (ActiveControlID == 50 ? ButtonHoveredColor : ButtonDefaultColor)
   , OverBackground = ButtonHoveredColor,
      PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Exit", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    MainMenuExitButton.Click += (s, a) => { Exit(); };

    MainMenuExitButton.MouseEntered += (s, a) =>
    {
      ActiveControlID = 50;
    };

    MainMenuVersionLabel = new Label
    {
      Text = "Version 1.4.1", Font = _fontSystem.GetFont(25), Left = 0, Top = screen_height_full - 30,
      TextColor = Color.Maroon, HorizontalAlignment = HorizontalAlignment.Center,
    };

    MainMenuTitleLabel = new Label
    {
      Text = "Polly vs Crows", Font = _fontSystem.GetFont(35), Left = 0, Top = 100,
      HorizontalAlignment = HorizontalAlignment.Center, TextColor = Color.Purple
    };

    grid.Widgets.Add(MainMenuStartButton);
    grid.Widgets.Add(MainMenuOptionsButton);
    grid.Widgets.Add(MainMenuManualButton);
    grid.Widgets.Add(MainMenuCreditsButton);
    grid.Widgets.Add(MainMenuExitButton);
    grid.Widgets.Add(MainMenuVersionLabel);
    grid.Widgets.Add(MainMenuTitleLabel);
  }

  public void HideAndDisposeMainMenuControls()
  {
    MainMenuStartButton.Visible = false;
    MainMenuOptionsButton.Visible = false;
    MainMenuManualButton.Visible = false;
    MainMenuCreditsButton.Visible = false;
    MainMenuExitButton.Visible = false;
    MainMenuVersionLabel.Visible = false;
    MainMenuTitleLabel.Visible = false;

    MainMenuStartButton = null;
    MainMenuOptionsButton = null;
    MainMenuManualButton = null;
    MainMenuCreditsButton = null;
    MainMenuExitButton = null;
    MainMenuVersionLabel = null;
    MainMenuTitleLabel = null;
  }
    public void CreateGameOverScreenControls()
  {
    GameOverScreenTitleLabel = new Label
    {
      Text = "GAME OVER", Font = _fontSystem.GetFont(30), Left = 310, Top = 200, TextColor = Color.Purple
    };

    GameOverScreenRestartButton = new Button
    {
      Left = 310, Top = 250,
      Width = screen_width - (2 * 310),
      Height = 40,
      BorderThickness = new Thickness(1),
      Background = ButtonDefaultColor,
      OverBackground = ButtonHoveredColor,
      PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Restart", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    GameOverScreenRestartButton.Click += (s, a) =>
    {
      PrepareGame();
      CurGameState = GameState.Playing;
    };

    GameOverScreenMainMenuButton = new Button
    {
      Left = 310, Top = 300,
      Width = screen_width - (2 * 310),
      Height = 40,
      BorderThickness = new Thickness(1),
      Background = ButtonDefaultColor,
      OverBackground = ButtonHoveredColor,
      PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Main menu", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    GameOverScreenMainMenuButton.Click += (s, a) =>
    {
      CurGameState = GameState.MainMenu;
    };

    GameOverScreenExitButton = new Button
    {
      Left = 310, Top = 350,
      Width = screen_width - (2 * 310),
      Height = 40,
      BorderThickness = new Thickness(1),
      Background = ButtonDefaultColor,
      OverBackground = ButtonHoveredColor,
      PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Exit", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    GameOverScreenExitButton.Click += (s, a) => { HideAndDisposeGameOverScreenControls(); Exit(); };

    grid.Widgets.Add(GameOverScreenTitleLabel);
    grid.Widgets.Add(GameOverScreenRestartButton);
    grid.Widgets.Add(GameOverScreenMainMenuButton);
    grid.Widgets.Add(GameOverScreenExitButton);
  }

  public void HideAndDisposeGameOverScreenControls()
  {
    GameOverScreenExitButton.Visible = false;
    GameOverScreenRestartButton.Visible = false;
    GameOverScreenMainMenuButton.Visible = false;
    GameOverScreenTitleLabel.Visible = false;

    GameOverScreenExitButton = null;
    GameOverScreenRestartButton = null;
    GameOverScreenMainMenuButton = null;
    GameOverScreenTitleLabel = null;
  }

  public void CreateCreditsScreenControls()
  {

    CreditsScreenMainMenuButton = new Button
    {
      Width = 150, Height = 40,
      Left = (screen_width - 150) / 2, Top = screen_height_playarea - 20,
      BorderThickness = new Thickness(1),
      Background = ButtonDefaultColor,
      OverBackground = ButtonHoveredColor,
      PressedBackground = ButtonDefaultColor,

      Content = new Label
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Text = "Main menu", Font = _fontSystem.GetFont(30),
        TextColor = Color.Black
      }
    };

    CreditsScreenMainMenuButton.Click += (s, a) => { CurGameState = GameState.MainMenu; };

    string sText = "Inspired by Color Chaos game by Daniel Rile" + (char)10;
    sText = sText + "https:///store.steampowered.com/app/423720/Color_Chaos/";

    sText = sText + (char)10 + (char)13 + (char)10;
    sText = sText + "Kyattou Ninden Teyandee / キャッ党 忍伝 てやんでえ" + (char)10;
    sText = sText + "made by Tatsunoko Productions and Sotsu Agency";
    sText = sText + (char)10 + (char)13 + (char)10;

    sText = sText + "Samurai Pizza Cats  - (c) Saban Entertainment " + (char)10;
    sText = sText + (char)10;

    sText = sText + "Font: FontStashSharp" + (char)10;
    sText = sText + "https:///github.com/FontStashSharp/FontStashSharp";
    sText = sText + (char)10 + (char)13 + (char)10;
    sText = sText + "Coding: Princess Luna" + (char)10;
    sText = sText + "Sprites provided by Meowzma O'tool" + (char)10;
    sText = sText + (char)10 + (char)13 + (char)10 + (char)13;
    sText = sText + "Created using Monogame 3.8.1" + (char)10;

    CreditsScreenTextLabel = new Label
    {
      Text = sText, Font = _fontSystem.GetFont(25), Left = 10, Top = 10, Width = screen_width - 10,
      TextColor = Color.Black
    };
    grid.Widgets.Add(CreditsScreenMainMenuButton);
    grid.Widgets.Add(CreditsScreenTextLabel);
  }
  public void HideAndDisposeCreditsScreenControls()
  {
    CreditsScreenTextLabel.Visible = false;
    CreditsScreenMainMenuButton.Visible = false;

    CreditsScreenMainMenuButton = null;
    CreditsScreenTextLabel = null;
  }

  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    _graphics.PreferredBackBufferHeight = 600;
    //_graphics.ToggleFullScreen();
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
  }

  protected override void Initialize()
  {

    ObjSettings = new ObjSettingsClass();
    quadenemies = new QuadtreeEnemies(1, new System.Drawing.RectangleF(0, 0, screen_width, screen_height_playarea));

    bool bool_;
    byte byte_;

    if (!File.Exists("Settings.ini"))
    {
      throw new FileNotFoundException("Settings.ini not found");
    }

    string sTmp;
    iniFile = new IniFile("Settings.ini");

    #region AutoShoot
    sTmp = iniFile.ReadString("AutoShoot", "Main", 255, "True");

    try
    {
      bool_ = bool.Parse(sTmp);
      ObjSettings.AutoShoot = bool_;
    }
    catch
    {
      ObjSettings.AutoShoot = true;
    }
    #endregion AutoShoot

    #region EnemySpawnInterval
    sTmp = iniFile.ReadString("EnemySpawnInterval", "Main", 255, "5");

    try
    {
      bool_ = Byte.TryParse(sTmp, out byte_);
      byte_ = Math.Clamp(byte_, (byte)5, (byte)5);
    }

    catch (Exception)
    {
      byte_ = 5;
    }

    ObjSettings.EnemySpawnInterval = byte_;
    #endregion EnemySpawnInterval

    #region EnemySpawnNumberMin
    sTmp = iniFile.ReadString("EnemySpawnNumberMin", "Main", 255, "3");

    try
    {
      bool_ = Byte.TryParse(sTmp, out byte_);
      byte_ = Math.Clamp(byte_, (byte)3, (byte)3);
    }

    catch (Exception)
    {
      byte_ = 3;
    }

    ObjSettings.EnemySpawnNumberMin = byte_;
    #endregion EnemySpawnNumberMin

    #region EnemySpawnNumberMax
    sTmp = iniFile.ReadString("EnemySpawnNumberMax", "Main", 255, "6");

    try
    {
      bool_ = Byte.TryParse(sTmp, out byte_);
      byte_ = Math.Clamp(byte_, (byte)6, (byte)6);
    }

    catch (Exception)
    {
      byte_ = 6;
    }

    ObjSettings.EnemySpawnNumberMax = byte_;
    #endregion EnemySpawnNumberMax

    #region PollyInvulnerabilityPeriod
    sTmp = iniFile.ReadString("PollyInvulnerabilityPeriod", "Main", 255, "3");

    try
    {
      bool_ = Byte.TryParse(sTmp, out byte_);
      byte_ = Math.Clamp(byte_, (byte)3, (byte)3);
    }

    catch (Exception)
    {
      byte_ = 3;
    }

    ObjSettings.PollyInvulnerabilityPeriod = byte_;

    #endregion PollyInvulnerabilityPeriod

    #region NumberOfKilledEnemiesToSpawnHealth
    sTmp = iniFile.ReadString("NumberOfKilledEnemiesToSpawnHealth", "Main", 255, "15");

    try
    {
      bool_ = Byte.TryParse(sTmp, out byte_);
      byte_ = Math.Clamp(byte_, (byte)15, (byte)15);
    }

    catch (Exception)
    {
      byte_ = 15;
    }

    ObjSettings.NumberOfKilledEnemiesToSpawnHealth = byte_;
    #endregion NumberOfKilledEnemiesToSpawnHealth

    #region MaxPlayerHealth
    sTmp = iniFile.ReadString("MaxPlayerHealth", "Main", 255, "3");
    try
    {
      bool_ = Byte.TryParse(sTmp, out byte_);
      byte_ = Math.Clamp(byte_, (byte)3, (byte)5);
    }

    catch (Exception)
    {
      byte_ = 3;
    }

    ObjSettings.MaxPlayerHealth = byte_;
    #endregion MaxPlayerHealth

    #region HealthVisibilityTime
    sTmp = iniFile.ReadString("HealthVisibilityTime", "Main", 255, "10");
    try
    {
      bool_ = Byte.TryParse(sTmp, out byte_);
      byte_ = Math.Clamp(byte_, (byte)5, (byte)20);
    }

    catch (Exception)
    {
      byte_ = 10;
    }

    ObjSettings.HealthVisibilityTime = byte_;
    #endregion HealthVisibilityTime

    #region DontHideHealth
    sTmp = iniFile.ReadString("DontHideHealth", "Main", 255, "True");

    try
    {
      bool_ = bool.Parse(sTmp);
      ObjSettings.AutoShoot = bool_;
    }
    catch
    {
      ObjSettings.AutoShoot = true;
    }
    #endregion DontHideHealth

    #region KeyboardAndMouseControl
    sTmp = iniFile.ReadString("PlayerControl", "Main", 255, "KeyboardandMouse");


    if (sTmp == "KeyboardandMouse")
    {
      ObjSettings.PlayerControl = PlayerControlDevice.KeyboardandMouse;
    }

    else if (sTmp == "Gamepad")
    {
      ObjSettings.PlayerControl = PlayerControlDevice.Gamepad;
    }

    else
    {
      ObjSettings.PlayerControl = PlayerControlDevice.KeyboardandMouse;
    }
    #endregion KeyboardAndMouseControl

    LevelInfoRectangle = new Rectangle(0, 0, screen_width, 25);

    ListOfEnemies = new List<EnemySprite>();
    ListOfPlayerBullets = new List<PlayerBulletSprite>();
    ListOfLostPlayerBullets = new List<PlayerBulletSprite>();
    ListOfKilledEnemies = new List<EnemySprite>();
    ListOfEnemyBullets = new List<EnemyBulletSprite>();
    ListOfLostEnemyBullets = new List<EnemyBulletSprite>();

    //screen_width = 800;//_graphics.PreferredBackBufferWidth;
    //screen_height = 480;//_graphics.PreferredBackBufferHeight;

    currentTime1 = 0;
    currentTime2 = 0;
    currentTime3 = 0;
    currentTime4 = 0;

    UpdateCount = 0;

    CurGameState = GameState.MainMenu;
    GameArea = new Rectangle(0, 25, screen_width, screen_height_full - 25);

    LastLeftMouseButtonState = ButtonState.Released;
    MouseState = new MouseState();
    MousePosition = new Point(0, 0);

    dictPollyDirectionSprites = new Dictionary<string, Texture2D>();
    dictCrowDirectionSprites = new Dictionary<string, Texture2D>();
    dictElectroDirectionSprites = new Dictionary<string, Texture2D>();
    EnemiesKilledSinceHPTakenOrHidden = 0;

    base.Initialize();
  }

  protected override void LoadContent()
  {
    Texture2D Texture_;

    _spriteBatch = new SpriteBatch(GraphicsDevice);

    HpHeartTexture = Content.Load<Texture2D>("Sprites/Sprite_Heart_HP");

    #region Sprite_Polly
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Left_1");
    dictPollyDirectionSprites["Left0"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Left_1");
    dictPollyDirectionSprites["Left1"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Left_2");
    dictPollyDirectionSprites["Left2"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Left_3");
    dictPollyDirectionSprites["Left3"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Left_4");
    dictPollyDirectionSprites["Left4"] = Texture_;

    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Right_1");
    dictPollyDirectionSprites["Right0"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Right_1");
    dictPollyDirectionSprites["Right1"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Right_2");
    dictPollyDirectionSprites["Right2"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Right_3");
    dictPollyDirectionSprites["Right3"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Right_4");
    dictPollyDirectionSprites["Right4"] = Texture_;

    #endregion Sprite_Polly

    #region Sprite_Crow
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Crow_Left_1");
    dictCrowDirectionSprites["Left1"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Crow_Left_2");
    dictCrowDirectionSprites["Left2"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Crow_Left_3");
    dictCrowDirectionSprites["Left3"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Crow_Left_4");
    dictCrowDirectionSprites["Left4"] = Texture_;

    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Crow_Right_1");
    dictCrowDirectionSprites["Right1"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Crow_Right_2");
    dictCrowDirectionSprites["Right2"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Crow_Right_3");
    dictCrowDirectionSprites["Right3"] = Texture_;
    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Crow_Right_4");
    dictCrowDirectionSprites["Right4"] = Texture_;
    #endregion Sprite_Crow

    #region dictElectroDirectionSprites


    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Electro_2");
    dictElectroDirectionSprites["1"] = Texture_;

    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Electro_1");
    dictElectroDirectionSprites["2"] = Texture_;

    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Electro_2");
    dictElectroDirectionSprites["3"] = Texture_;

    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Electro_3");
    dictElectroDirectionSprites["4"] = Texture_;

    Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Electro_Attacking");
    dictElectroDirectionSprites["Attacking"] = Texture_;

    #endregion dictElectroDirectionSprites

    ObjPlayer = new PlayerSprite(dictPollyDirectionSprites["Right1"],
    new Vector2(screen_width / 2, screen_height_full / 2), "Right");
    ObjPlayer.Health = ObjSettings.MaxPlayerHealth;

    ObjHpHeart = new HpHeartSprite(HpHeartTexture, new Vector2(10, 10));
    ObjHpHeart.Visible = false;

    _fontSystem = new FontSystem();
    _fontSystem.AddFont(File.ReadAllBytes(@"Content/Fonts/DroidSans.ttf"));
    _fontSystem.AddFont(File.ReadAllBytes(@"Content/Fonts/DroidSansJapanese.ttf"));

    MyraEnvironment.Game = this;
    grid = new Grid { };
    _desktop = new Desktop();
    _desktop.Root = grid;

    CreateMainMenuControls();

    
  }

  protected override void Update(GameTime gameTime)
  {
    float DeltaX, DeltaY;
    float abs_dx, abs_dy;

    float LeftStickX = 0;
    float LeftStickY = 0;
    float RightStickX = 0;
    float RightStickY = 0;

    string sTmp;
    Double Atan2R;
    Texture2D Texture_;

    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
    {
      CurGameState = GameState.MainMenu;
    }

    GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
    GamePadState gpstate = GamePad.GetState(PlayerIndex.One);

    switch (CurGameState)
    {
      case GameState.MainMenu:

        currentTime1 = currentTime1 + (float)gameTime.ElapsedGameTime.TotalSeconds;

        LeftStickY = gpstate.ThumbSticks.Left.Y;

        if (gameTime.TotalGameTime.TotalSeconds - 0.15 >= LastLeftDpadCheckTime)
        {
          if (LeftStickY < 0)
          {
            if (ActiveControlID == 0) { ActiveControlID = 10; return; }
            if (ActiveControlID == 50) { return; }

            this.IsMouseVisible = false;
            ActiveControlID = ActiveControlID + 10;
          }
          else if (LeftStickY > 0)
          {
            if (ActiveControlID == 0) { ActiveControlID = 10; return; }
            if (ActiveControlID == 10) { return; }

            this.IsMouseVisible = false;
            ActiveControlID = ActiveControlID - 10;
          }

          if (gpstate.DPad.Down == ButtonState.Pressed)
          {
            if (ActiveControlID == 0) { ActiveControlID = 10; return; }
            if (ActiveControlID == 50) { return; }

            this.IsMouseVisible = false;
            ActiveControlID = ActiveControlID + 10;
          }
          else if (gpstate.DPad.Up == ButtonState.Pressed)
          {
            if (ActiveControlID == 0) { ActiveControlID = 10; return; }
            if (ActiveControlID == 10) { return; }

            ActiveControlID = ActiveControlID - 10;
            this.IsMouseVisible = false;
          }

          LastLeftDpadCheckTime = gameTime.TotalGameTime.TotalSeconds;
        }

        if (gpstate.Buttons.X == ButtonState.Pressed)
        {
          if (ActiveControlID == 10) { MainMenuStartButton.DoClick(); }
          else if (ActiveControlID == 20) { MainMenuOptionsButton.DoClick(); }
          else if (ActiveControlID == 30) { MainMenuManualButton.DoClick(); }
          else if (ActiveControlID == 40) { MainMenuCreditsButton.DoClick(); }
          else if (ActiveControlID == 50) { MainMenuExitButton.DoClick(); }
        }

        break;

      case GameState.Credits:
        MouseState = Mouse.GetState();
        MousePosition = new Point(MouseState.X, MouseState.Y);

        LastLeftMouseButtonState = CurLeftMouseButtonState;
        CurLeftMouseButtonState = Mouse.GetState().LeftButton;
        break;

      //GameState.Credits END   

      case GameState.GameOver:
        MouseState = Mouse.GetState();
        MousePosition = new Point(MouseState.X, MouseState.Y);

        LastLeftMouseButtonState = CurLeftMouseButtonState;
        CurLeftMouseButtonState = Mouse.GetState().LeftButton;
        break;
      //GameState.GameOver END 

      case GameState.Options:

        LeftStickY = gpstate.ThumbSticks.Left.Y;
        LeftStickX = gpstate.ThumbSticks.Left.X;

        CurLeftMouseButtonState = Mouse.GetState().LeftButton;

       // currentTime1 = currentTime1 + (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (gameTime.TotalGameTime.TotalSeconds - 0.15 >= LastLeftDpadCheckTime)
        {

          if ((LeftStickY < 0) || (gpstate.DPad.Down == ButtonState.Pressed))
        {
          if (ActiveControlID < 100) { ActiveControlID = ActiveControlID + 10; }
        }
        else if ((LeftStickY > 0) || (gpstate.DPad.Up == ButtonState.Pressed))
        {
          if (ActiveControlID == 105) { ActiveControlID = 100; return; }
          if (ActiveControlID > 15) { ActiveControlID = ActiveControlID - 10; }
        }

        //  LastLeftDpadCheckTime = gameTime.TotalGameTime.TotalSeconds;


      //  currentTime1 = currentTime1 + (float)gameTime.ElapsedGameTime.TotalSeconds;


          if (LeftStickX < 0)
        {
          DeltaX = -1;

          switch (ActiveControlID)
          {
            case 20: hsEnemySpawnInterval.Value = hsEnemySpawnInterval.Value + DeltaX; break;
            case 30: hsEnemySpawnNumberMin.Value = hsEnemySpawnNumberMin.Value + DeltaX; break;
            case 40: hsEnemySpawnNumberMax.Value = hsEnemySpawnNumberMax.Value + DeltaX; break;
            case 50: hsPollyInvulnerabilityPeriod.Value = hsPollyInvulnerabilityPeriod.Value + DeltaX; break;
            case 60: hsNumberOfKilledEnemiesToSpawnHealth.Value = hsNumberOfKilledEnemiesToSpawnHealth.Value + DeltaX; break;
            case 70: hsMaxPlayerHealth.Value = hsMaxPlayerHealth.Value + DeltaX; break;
            case 80: hsHealthVisibilityTime.Value = hsHealthVisibilityTime.Value + DeltaX; break;
            case 105: ActiveControlID = 100; break;
            default: break;
          }
        }

        if (LeftStickX > 0)
        {
          DeltaX = 1;

          switch (ActiveControlID)
          {
            case 20: hsEnemySpawnInterval.Value = hsEnemySpawnInterval.Value + DeltaX; break;
            case 30: hsEnemySpawnNumberMin.Value = hsEnemySpawnNumberMin.Value + DeltaX; break;
            case 40: hsEnemySpawnNumberMax.Value = hsEnemySpawnNumberMax.Value + DeltaX; break;
            case 50: hsPollyInvulnerabilityPeriod.Value = hsPollyInvulnerabilityPeriod.Value + DeltaX; break;
            case 60: hsNumberOfKilledEnemiesToSpawnHealth.Value = hsNumberOfKilledEnemiesToSpawnHealth.Value + DeltaX; break;
            case 70: hsMaxPlayerHealth.Value = hsMaxPlayerHealth.Value + DeltaX; break;
            case 80: hsHealthVisibilityTime.Value = hsHealthVisibilityTime.Value + DeltaX; break;
            case 100: ActiveControlID = 105; break;
            default: break;
          }
        }

        if (gpstate.Buttons.X == ButtonState.Pressed)
        {
          if (ActiveControlID == 10) { chbAutoShoot.IsChecked = !chbAutoShoot.IsChecked; }
          if (ActiveControlID == 90) { chbDontHideHealth.IsChecked = !chbDontHideHealth.IsChecked; }
          if ((ActiveControlID == 100) && (!chbKeyboardAndMouseControl.IsChecked)) { chbKeyboardAndMouseControl.IsChecked = true; }
          if ((ActiveControlID == 105) && (!chbGamepadControl.IsChecked)) { chbGamepadControl.IsChecked = true; }

        }
          LastLeftDpadCheckTime = gameTime.TotalGameTime.TotalSeconds;

        }
        break;

      case GameState.Playing:
        UpdateCount = UpdateCount + 1;
        currentTime1 = currentTime1 + (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 
        currentTime2 = currentTime2 + (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 
        currentTime3 = currentTime3 + (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 

        if (ObjHpHeart.Visible)
        {
          currentTime4 = currentTime4 + (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 
        }

        // GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
        //   GamePadState gpstate= GamePad.GetState(PlayerIndex.One);

        if (currentTime1 >= EnemySpawnTimerInterval)
        {
          currentTime1 = 0f;
          SpawnEnemies();
        }

        if (currentTime2 >= ElectroFireInterval)
        {
          currentTime2 = 0f;
          ElectroEnemyFireLightning();
        }

        if (currentTime3 >= PollyInvulnerabilityInterval)
        {
          currentTime3 = 0f;
          ObjPlayer.IsInvulnerable = false;
        }

        if ((currentTime4 >= ObjHpHeart.AppearanceTime + ObjSettings.HealthVisibilityTime) && (ObjSettings.DontHideHealth == false))
        {
          currentTime4 = 0;
          ObjHpHeart.Visible = false;
          EnemiesKilledSinceHPTakenOrHidden = 0;
        }

        MouseState = Mouse.GetState();
        MousePosition = new Point(MouseState.X, MouseState.Y);

        if (MouseState.X >= ObjPlayer.position.X + (ObjPlayer.texture.Width / 2))
        {
          sTmp = "Right";
        }
        else
        {
          sTmp = "Left";
        }

        ObjPlayer.direction = sTmp;

        LastLeftMouseButtonState = CurLeftMouseButtonState;
        CurLeftMouseButtonState = Mouse.GetState().LeftButton;

        LastRightMouseButtonState = CurRightMouseButtonState;
        CurRightMouseButtonState = Mouse.GetState().RightButton;

        if ((NinpouPower == 100)
        &&
        (
          (
          (GameArea.Contains(MousePosition))
          && (CurRightMouseButtonState == ButtonState.Pressed)
          && (LastRightMouseButtonState == ButtonState.Released)
          && (ObjSettings.PlayerControl == PlayerControlDevice.KeyboardandMouse)
          )

          ||

          (
          (ObjSettings.PlayerControl == PlayerControlDevice.Gamepad)
          && (gpstate.IsButtonDown(Buttons.X) == true)
          )
        ))
        {
          NinpouPower = 0;
          Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Weapon_Ninpou");

          for (byte i = 0; i <= 7; i = (byte)(i + 1))
          {
            DeltaX = ListOfPlayerNinpoudx[i];
            DeltaY = ListOfPlayerNinpoudy[i];

            PlayerBulletSprite PlayerBullet = new PlayerBulletSprite(texture_: Texture_

            , position_: new Vector2(
            ObjPlayer.position.X + (float)0.5 * (ObjPlayer.texture.Height - Texture_.Height)
            , ObjPlayer.position.Y + (float)0.5 * (ObjPlayer.texture.Width - Texture_.Width))

            , deltax_: DeltaX
            , deltay_: DeltaY
            , type_: "Ninpou");

            ListOfPlayerBullets.Add(PlayerBullet);
          }
        }


        //   if (capabilities.IsConnected)
        //   {
        gpstate = GamePad.GetState(PlayerIndex.One);

        LeftStickX = gpstate.ThumbSticks.Left.X;
        LeftStickY = gpstate.ThumbSticks.Left.Y;

        RightStickX = (float)(Math.Round(gpstate.ThumbSticks.Right.X, 3));
        RightStickY = (float)(Math.Round(gpstate.ThumbSticks.Right.Y, 3));

        DeltaX = LeftStickX * 2;
        DeltaY = LeftStickY * 2 * -1; //Контроллер вниз (-1) это вверх по экрану  

        KeyboardState state = Keyboard.GetState();

        if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
        {
          DeltaX = DeltaX + 2;
        }

        if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
        {
          DeltaX = DeltaX - 2;
        }

        ObjPlayer.position.X = ObjPlayer.position.X + DeltaX;
        ObjPlayer.position.X = Math.Clamp(ObjPlayer.position.X, 0,
        screen_width - ObjPlayer.texture.Width * KNTGame.Game1.SpriteResizeKoof);

        if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
        {
          DeltaY = DeltaY - 2;
        }

        if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
        {
          DeltaY = DeltaY + 2;
        }

        ObjPlayer.position.Y = ObjPlayer.position.Y + DeltaY;

        ObjPlayer.position.Y = Math.Clamp(ObjPlayer.position.Y, 25,
        screen_height_full - ObjPlayer.texture.Height * KNTGame.Game1.SpriteResizeKoof);

        ObjPlayer.IsMoving = ((DeltaX != 0) || (DeltaY != 0));

        float TargetX = ObjPlayer.position.X;
        float TargetY = ObjPlayer.position.Y;

        //-----------------------------
        if (((RightStickX != 0.0f) || (RightStickY != 0.0f))
        && (gameTime.TotalGameTime.TotalSeconds - PlayerBulletInterval >= LastPlayerBulletShootTime)
        && (ObjSettings.PlayerControl == PlayerControlDevice.Gamepad))

        {
          LastPlayerBulletShootTime = gameTime.TotalGameTime.TotalSeconds;

          TargetX = TargetX + RightStickX;
          TargetY = TargetY + (-1 * RightStickY);

          Getdxdy(x1: ObjPlayer.position.X, y1: ObjPlayer.position.Y,
          x2: TargetX, y2: TargetY,
          dx: out DeltaX, dy: out DeltaY);

          /*        Atan2R = Math.Atan2(ObjPlayer.position.Y - TargetY, ObjPlayer.position.X - TargetX);

                    abs_dx = Math.Abs((float)Math.Cos(Atan2R));
                    abs_dy = Math.Abs((float)Math.Sin(Atan2R));

                    if (RightStickX > 0)
                    {
                      DeltaX = abs_dx;
                    }
                    else
                    {
                      DeltaX = -1 * abs_dx;
                    }

                    if (RightStickY < 0)
                    {
                      DeltaY = abs_dy;
                    }
                    else
                    {
                      DeltaY = -1 * abs_dy;
                    }
          */

          Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Weapon");

          PlayerBulletSprite PlayerBullet = new PlayerBulletSprite(texture_: Texture_
            , position_: new Vector2(
            ObjPlayer.position.X + (float)0.5 * (ObjPlayer.texture.Height - Texture_.Height)
            , ObjPlayer.position.Y + (float)0.5 * (ObjPlayer.texture.Width - Texture_.Width))
            , deltax_: DeltaX
            , deltay_: DeltaY
            , type_: "Normal");

          ListOfPlayerBullets.Add(PlayerBullet);
        }
        //------------------

        if (GameArea.Contains(MousePosition) && (ObjSettings.PlayerControl == PlayerControlDevice.KeyboardandMouse)
          && (gameTime.TotalGameTime.TotalSeconds - PlayerBulletInterval >= LastPlayerBulletShootTime)
          &&
          (
          ((CurLeftMouseButtonState == ButtonState.Pressed) && (ObjSettings.AutoShoot == false))
          || (ObjSettings.AutoShoot == true)))
        {

          LastPlayerBulletShootTime = gameTime.TotalGameTime.TotalSeconds;

          Getdxdy(x1: ObjPlayer.position.X, y1: ObjPlayer.position.Y,
          x2: MouseState.X, y2: MouseState.Y,
          dx: out DeltaX, dy: out DeltaY);

          /*
                    Atan2R = Math.Atan2(ObjPlayer.position.Y - MouseState.Y, ObjPlayer.position.X - MouseState.X);

                    abs_dx = Math.Abs((float)Math.Cos(Atan2R));
                    abs_dy = Math.Abs((float)Math.Sin(Atan2R));

                    if (ObjPlayer.position.X < MouseState.X)
                    {
                      DeltaX = abs_dx;
                    }
                    else
                    {
                      DeltaX = -1 * abs_dx;
                    }

                    if (ObjPlayer.position.Y < MouseState.Y)
                    {
                      DeltaY = abs_dy;
                    }
                    else
                    {
                      DeltaY = -1 * abs_dy;
                    }
          */

          Texture_ = Content.Load<Texture2D>("Sprites/Sprite_Polly_Weapon");

          PlayerBulletSprite PlayerBullet = new PlayerBulletSprite(texture_: Texture_
          , position_: new Vector2(
          ObjPlayer.position.X + (float)0.5 * (ObjPlayer.texture.Height - Texture_.Height)
          , ObjPlayer.position.Y + (float)0.5 * (ObjPlayer.texture.Width - Texture_.Width)
          )

          , deltax_: DeltaX
          , deltay_: DeltaY
          , type_: "Normal");

          ListOfPlayerBullets.Add(PlayerBullet);
        }

        foreach (EnemySprite sprite in ListOfEnemies)
        {
          Atan2R = Math.Atan2(sprite.position.Y - ObjPlayer.position.Y,
          sprite.position.X - ObjPlayer.position.X);

          abs_dx = Math.Abs((float)Math.Cos(Atan2R));
          abs_dy = Math.Abs((float)Math.Sin(Atan2R));

          if (sprite.position.X < ObjPlayer.position.X)
          {
            DeltaX = abs_dx;
            if (!sprite.CanGoRight) { DeltaX = 0; }
          }
          else
          {
            DeltaX = -1 * abs_dx;

            if (!sprite.CanGoLeft) { DeltaX = 0; }
          }

          if (sprite.position.Y < ObjPlayer.position.Y)
          {
            DeltaY = abs_dy;
            if (!sprite.CanGoDown) { DeltaY = 0; }
          }
          else
          {
            DeltaY = -1 * abs_dy;
            if (!sprite.CanGoUp) { DeltaY = 0; }
          }

          sprite.position.X = (sprite.position.X + DeltaX);
          sprite.position.Y = (sprite.position.Y + DeltaY);

          if (sprite.position.X + (sprite.texture.Width / 2)
          < (ObjPlayer.position.X + (ObjPlayer.texture.Width / 2)))
          {
            sTmp = "Right";
          }
          else
          {
            sTmp = "Left";
          }

          if ((sprite.Type == EnemyType.Crow)
          && (sprite.direction != sTmp))
          {
            (sprite as EnemySpriteCrow).LastDirectionChangeTime = UpdateCount;
          }

          sprite.direction = sTmp;
          sprite.CanGoRight = true;
          sprite.CanGoLeft = true;
          sprite.CanGoDown = true;
          sprite.CanGoUp = true;

          if ((sprite.Type == EnemyType.Electro)
          && ((sprite as EnemySpriteElectro).IsAttacking == true)
          && (UpdateCount >= ((sprite as EnemySpriteElectro).LastFireStateTriggerTime + 180)))
          {
            (sprite as EnemySpriteElectro).IsAttacking = false;
            (sprite as EnemySpriteElectro).LastFireStateTriggerTime = UpdateCount;
          }
        }

        #region 
        quadenemies.clear();

        foreach (EnemySprite esprite in ListOfEnemies)
        {
          quadenemies.insert(esprite);
        }

        List<EnemySprite> returnObjects = new List<EnemySprite>();

        foreach (EnemySprite esprite1 in ListOfEnemies)
        {
          returnObjects.Clear();
          quadenemies.retrieve(esprite1, returnObjects);

          foreach (EnemySprite esprite2 in returnObjects)
            if (esprite1 != esprite2)
            {
              if (esprite1.Rect.IntersectsWith(esprite2.Rect))
              {
                SetRectMoveLimits(esprite1, esprite2);
              }
            }

          if (esprite1.Rect.IntersectsWith(ObjPlayer.Rect))
          {
            if (ObjPlayer.IsInvulnerable)
            {
            }
            else
            {
              ObjPlayer.Health = (Byte)(ObjPlayer.Health - 1);

              if (ObjPlayer.Health == 0)
              {
                CurGameState = GameState.GameOver;
              }
              else
              {
                ObjPlayer.IsInvulnerable = true;
                ObjPlayer.InvulnerabilityStatusChangeTime = UpdateCount;
              }
            }
          }
        }
        #endregion 

        foreach (PlayerBulletSprite PlayerBullet in ListOfPlayerBullets)
        {
          PlayerBullet.Move();

          if (PlayerBullet.IsOutsideRoom())
          {
            ListOfLostPlayerBullets.Add(PlayerBullet);
          }

          foreach (EnemySprite EnemySprite_ in ListOfEnemies)
          {
            if (EnemySprite_.Rect.IntersectsWith(PlayerBullet.Rect))
            {
              if (PlayerBullet.type == "Normal")
              {
                EnemySprite_.DecreaseHP(1);
              }
              else
              {
                EnemySprite_.DecreaseHP(3);
              }

              if (EnemySprite_.HP <= 0)
              {
                ListOfKilledEnemies.Add(EnemySprite_);
                EnemiesKilled = (Int16)(EnemiesKilled + 1);

                if (ObjHpHeart.Visible == false)
                {
                  EnemiesKilledSinceHPTakenOrHidden = (Int16)(EnemiesKilledSinceHPTakenOrHidden + 1);
                }

                if ((EnemiesKilledSinceHPTakenOrHidden >= NumberOfKilledEnemiesToSpawnHealth)
               && (ObjHpHeart.Visible == false))
                {
                  SpawnHealthHeart();
                  ObjHpHeart.AppearanceTime = currentTime4;
                }

                if (NinpouPower < 100)
                {
                  NinpouPower = (byte)(NinpouPower + 10);
                }
              }
            }
          }
        }

        foreach (EnemySprite KilledEnemy in ListOfKilledEnemies)
        {
          ListOfEnemies.Remove(KilledEnemy);
        }

        ListOfKilledEnemies.Clear();

        foreach (PlayerBulletSprite LostPlayerBullet in ListOfLostPlayerBullets)
        {
          ListOfPlayerBullets.Remove(LostPlayerBullet);
        }

        ListOfLostPlayerBullets.Clear();

        foreach (EnemyBulletSprite EnemyBullet in ListOfEnemyBullets)
        {
          EnemyBullet.Move();

          if (EnemyBullet.IsOutsideRoom())
          {
            ListOfLostEnemyBullets.Add(EnemyBullet);
          }

          if (EnemyBullet.Rect.IntersectsWith(ObjPlayer.Rect))
          {
            if (ObjPlayer.IsInvulnerable)
            {
            }
            else
            {
              ListOfLostEnemyBullets.Add(EnemyBullet);
              ObjPlayer.Health = (Byte)(ObjPlayer.Health - 1);

              if (ObjPlayer.Health <= 0)
              {
                CurGameState = GameState.GameOver;
              }
              else
              {
                ObjPlayer.IsInvulnerable = true;
                ObjPlayer.InvulnerabilityStatusChangeTime = UpdateCount;
              }
            }
          }
        }

        foreach (EnemyBulletSprite LostEnemyBullet in ListOfLostEnemyBullets)
        {
          ListOfEnemyBullets.Remove(LostEnemyBullet);
        }

        ListOfLostEnemyBullets.Clear();

        if (ObjPlayer.Rect.IntersectsWith(ObjHpHeart.Rect)
        && (ObjHpHeart.Visible))
        {
          EnemiesKilledSinceHPTakenOrHidden = 0;
          ObjHpHeart.AppearanceTime = 0;
          ObjHpHeart.Visible = false;
          ObjPlayer.Health = ObjSettings.MaxPlayerHealth;
          currentTime4 = 0;
        }

        LevelInfoLabel1.Text = currentTime1.ToString();
        base.Update(gameTime);
        //GameState.Playing END
        break;

    }
  }

  protected override void Draw(GameTime gameTime)
  {

    this.IsMouseVisible =
         ((CurGameState != GameState.Playing)
     || (CurGameState == GameState.Playing) && (ObjSettings.PlayerControl == PlayerControlDevice.KeyboardandMouse));

    switch (CurGameState)
    {
      case GameState.Credits:

        GraphicsDevice.Clear(Color.CornflowerBlue);
        _desktop.Render();
        _spriteBatch.Begin();

        _spriteBatch.End();
        base.Draw(gameTime);
        break;

      case GameState.MainMenu:

        GraphicsDevice.Clear(Color.CornflowerBlue);
        _desktop.Render();
        _spriteBatch.Begin();

        MainMenuStartButton.Background = (ActiveControlID == 10 ? ButtonHoveredColor : ButtonDefaultColor);
        MainMenuStartButton.OverBackground = (ActiveControlID == 10 ? ButtonHoveredColor : ButtonDefaultColor);

        MainMenuOptionsButton.Background = (ActiveControlID == 20 ? ButtonHoveredColor : ButtonDefaultColor);
        MainMenuOptionsButton.OverBackground = (ActiveControlID == 20 ? ButtonHoveredColor : ButtonDefaultColor);

        MainMenuManualButton.Background = (ActiveControlID == 30 ? ButtonHoveredColor : ButtonDefaultColor);
        MainMenuManualButton.OverBackground = (ActiveControlID == 30 ? ButtonHoveredColor : ButtonDefaultColor);

        MainMenuCreditsButton.Background = (ActiveControlID == 40 ? ButtonHoveredColor : ButtonDefaultColor);
        MainMenuCreditsButton.OverBackground = (ActiveControlID == 40 ? ButtonHoveredColor : ButtonDefaultColor);

        MainMenuExitButton.Background = (ActiveControlID == 50 ? ButtonHoveredColor : ButtonDefaultColor);
        MainMenuExitButton.OverBackground = (ActiveControlID == 50 ? ButtonHoveredColor : ButtonDefaultColor);

      //  MainMenuVersionLabel.Text = "Version 1.4.1";
        _spriteBatch.End();
        base.Draw(gameTime);
        break;

      case GameState.Playing:

        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        foreach (EnemySprite EnemySprite_ in ListOfEnemies)
        {
          if (EnemySprite_.Type == EnemyType.Crow)
          {
            _spriteBatch.Draw(
            dictCrowDirectionSprites[
            EnemySprite_.direction + GetSpriteFrameNumber2(UpdateCount - (EnemySprite_ as EnemySpriteCrow).LastDirectionChangeTime)], EnemySprite_.position, null, Color.White, 0f,
            Vector2.Zero, KNTGame.Game1.SpriteResizeKoof, SpriteEffects.None, 0.0f);
          }
          else
          {
            if ((EnemySprite_ as EnemySpriteElectro).IsAttacking == true)
            {
              _spriteBatch.Draw(dictElectroDirectionSprites["Attacking"], EnemySprite_.position, null, Color.White, 0f, Vector2.Zero, KNTGame.Game1.SpriteResizeKoof, SpriteEffects.None, 0.0f);
            }
            else
            {
              _spriteBatch.Draw(
              dictElectroDirectionSprites[
              GetSpriteFrameNumber1(UpdateCount - (EnemySprite_ as EnemySpriteElectro).LastFireStateTriggerTime)], EnemySprite_.position, null,
              Color.White, 0f,
              Vector2.Zero, KNTGame.Game1.SpriteResizeKoof, SpriteEffects.None, 0.0f);
            }
          }
        }

        foreach (PlayerBulletSprite sprite in ListOfPlayerBullets)
        {
          //_spriteBatch.Draw(sprite.texture, sprite.position, Color.White);
          _spriteBatch.Draw(sprite.texture, sprite.position, null, Color.White, 0f, Vector2.Zero, KNTGame.Game1.SpriteResizeKoof, SpriteEffects.None, 0.0f);
        }

        foreach (EnemyBulletSprite EnemyBullet in ListOfEnemyBullets)
        {
          //_spriteBatch.Draw(sprite.texture, sprite.position, Color.White);
          _spriteBatch.Draw(EnemyBullet.texture, EnemyBullet.position, null, Color.White, 0f, Vector2.Zero, KNTGame.Game1.SpriteResizeKoof, SpriteEffects.None, 0.0f);
        }

        float TransparencyKoeff;

        if (ObjPlayer.IsInvulnerable)
        {
          TransparencyKoeff = 0.65f;
        }
        else
        {
          TransparencyKoeff = 1.0f;
        }

        _spriteBatch.Draw(dictPollyDirectionSprites[GetKeyForPollyDirectionDictionary()], ObjPlayer.position,
        null, Color.White * TransparencyKoeff, 0f,
        Vector2.Zero, KNTGame.Game1.SpriteResizeKoof, SpriteEffects.None, 0.0f);

        if (ObjHpHeart.Visible)
        {
          _spriteBatch.Draw(ObjHpHeart.texture, ObjHpHeart.position, null, Color.White, 0f,
          Vector2.Zero, 1, SpriteEffects.None, 0.0f);
        }

         _spriteBatch.FillRectangle(LevelInfoRectangle, Color.Gray);

        _spriteBatch.DrawString(_fontSystem.GetFont(22), "Enemies Killed = " + EnemiesKilled.ToString(), new Vector2(10, 5), Color.LightGreen);
        _spriteBatch.DrawString(_fontSystem.GetFont(22), "Health = " + ObjPlayer.Health.ToString(), new Vector2((screen_width / 2) - 50, 5), Color.LightGreen, layerDepth: 0);
        _spriteBatch.DrawString(_fontSystem.GetFont(22), "Ninpou Power = " + NinpouPower.ToString(), new Vector2(screen_width - 200, 5), Color.LightGreen, layerDepth: 0);

        _spriteBatch.End();

        base.Draw(gameTime);
        break;

      case GameState.GameOver:

        GraphicsDevice.Clear(Color.CornflowerBlue);
        _desktop.Render();
        _spriteBatch.Begin();

        _spriteBatch.FillRectangle(LevelInfoRectangle, Color.Gray, 0);
        _spriteBatch.DrawString(_fontSystem.GetFont(22), "Enemies Killed = " + EnemiesKilled.ToString(), new Vector2(10, 5), Color.LightGreen);
        _spriteBatch.DrawString(_fontSystem.GetFont(22), "Health = " + ObjPlayer.Health.ToString(), new Vector2((screen_width / 2) - 150, 5), Color.LightGreen, layerDepth: 0);
        _spriteBatch.DrawString(_fontSystem.GetFont(22), "Ninpou Power = " + NinpouPower.ToString(), new Vector2(screen_width - 200, 5), Color.LightGreen, layerDepth: 0);

        _spriteBatch.End();
        base.Draw(gameTime);
        break;

      case GameState.Options:
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _desktop.Render();
        _spriteBatch.Begin();

     //   OptionsMenuLabel1.Text = hsEnemySpawnInterval.Value.ToString();
        OptionsMenuLabel1.TextColor = (ActiveControlID == 10 ? LabelActiveColor : Color.Black);
        OptionsMenuLabel21.TextColor = (ActiveControlID == 20 ? LabelActiveColor : Color.Black);
        OptionsMenuLabel31.TextColor = (ActiveControlID == 30 ? LabelActiveColor : Color.Black);
        OptionsMenuLabel41.TextColor = (ActiveControlID == 40 ? LabelActiveColor : Color.Black);
        OptionsMenuLabel51.TextColor = (ActiveControlID == 50 ? LabelActiveColor : Color.Black);
        OptionsMenuLabel61.TextColor = (ActiveControlID == 60 ? LabelActiveColor : Color.Black);
        OptionsMenuLabel71.TextColor = (ActiveControlID == 70 ? LabelActiveColor : Color.Black);
        OptionsMenuLabel81.TextColor = (ActiveControlID == 80 ? LabelActiveColor : Color.Black);

        OptionsMenuLabel92.TextColor = (ActiveControlID == 100 ? LabelActiveColor : Color.Black);
        OptionsMenuLabel93.TextColor = (ActiveControlID == 105 ? LabelActiveColor : Color.Black);

        _spriteBatch.DrawString(_fontSystem.GetFont(30), "OPTIONS", new Vector2(screen_width / 2 - 40, 10), Color.Black);
        _spriteBatch.End();

        base.Draw(gameTime);
        break;

      case GameState.Manual:
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        _desktop.Render();
        _spriteBatch.End();
        base.Draw(gameTime);
        break;
    }
  }
}
