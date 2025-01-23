using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;

namespace PaintTogether;

public class Game1 : Game
{
    public SpriteBatch SpriteBatch { get; private set; }
    public Mode CurrMode { get; private set; }
    private GraphicsDeviceManager _graphics;
    private Desktop _desktop;
    
    private Mainmenu? _mainMenu;
    private Server? _host;
    private Client? _client;
    private JoinForm? _joinForm;

    public Game1()
    {
        CurrMode = Mode.NONE;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 640;
        _graphics.PreferredBackBufferHeight = 480;
        _graphics.ApplyChanges();
            
        SwitchMode(Mode.MAIN_MENU);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkSlateBlue);
        // _desktop.Render();

        SpriteBatch.Begin();
        base.Draw(gameTime);
        SpriteBatch.End();
    }

    public void SwitchMode(Mode mode, string ip = "")
    {
        if (mode == CurrMode) return;
        
        if (_mainMenu != null)
        {
            Components.Remove(_mainMenu);
            _mainMenu.Dispose();
            _mainMenu = null;
        }
        if (_host != null) 
        {
            Components.Remove(_host);
            _host.Dispose();
            _host = null;
        }
        if (_client != null) 
        {
            Components.Remove(_client);
            _client.Dispose();
            _client = null;
        }
        if (_joinForm != null) 
        {
            Components.Remove(_joinForm);
            _joinForm.Dispose();
            _joinForm = null;
        }
        
        switch (mode)
        {
            case Mode.MAIN_MENU:
                _mainMenu = new (this);
                Components.Add(_mainMenu);
                break;
            case Mode.HOST:
                _host = new (this);
                Components.Add(_host);
                break;
            case Mode.CLIENT:
                _client = new(this, ip);
                Components.Add(_client);
                break;
            case Mode.JOIN_FORM:
                _joinForm = new(this);
                Components.Add(_joinForm);
                break;
            default:
                Console.WriteLine("Invalid Mode");
                return;
        }
        CurrMode = mode;
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
