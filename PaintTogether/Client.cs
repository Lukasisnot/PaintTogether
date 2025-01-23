using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace PaintTogether;

public class Client : DrawableGameComponent
{
    private readonly Game1 _game;
    private GameView _gameView;
    private string _ipLocal;
    private string _ipHost;
    private readonly Socket _socket = new (SocketType.Stream, ProtocolType.Tcp);

    private bool _isDisconnected = false;
    
    public Client(Game1 game, string ipHost) : base(game)
    {
        _game = game;
        _ipHost = ipHost;
        _ipLocal = _game.GetLocalIPAddress();
        _socket.SendTimeout = 500;
        _socket.ReceiveTimeout = 100;
    }

    public override void Initialize()
    {
        base.Initialize();
        if (!Connect())
        {
            Console.WriteLine("Failed to connect");
            _game.SwitchMode(Mode.MAIN_MENU);
            return;
        }
        
        _gameView = new GameView(_game,  $"Connected to: {_ipHost}");
        _game.Components.Add(_gameView);
        
        _gameView.Whiteboard.AddLayer();
        _gameView.Whiteboard.SwitchLayerDrawDir = true;
        
        Task.Run(Receive);
        Task.Run(Send);
    }

    private bool Connect()
    {
        try
        {
            _socket.Connect(IPAddress.Parse(_ipHost), 26666);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while connecting: {ex.Message}");
            return false;
        }
        return true;
    }
    
    private bool Disconnect()
    {
        if (_isDisconnected) return true;
        try
        {
            _game.SwitchMode(Mode.MAIN_MENU);
            _isDisconnected = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while disconnecting: {ex.Message}");
            return false;
        }
        return true;
    }

    private async Task Receive()
    {
        while (true)
        {
            try
            {
                byte[] serverBuffer = new byte[_gameView.Whiteboard.Width * _gameView.Whiteboard.Height * 4];
                int bytesReceived = await _socket.ReceiveAsync(serverBuffer);
                
                if (bytesReceived != serverBuffer.Length) continue;
                
                _gameView.Whiteboard.DrawToLayer(0, serverBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while receiving: {ex.Message}, disconnecting...");
                Disconnect();
                return;
            }
            Thread.Sleep(10);
        }
    }

    private async Task Send()
    {
        while (true)
        {
            try
            {
                int bytesSent = 0;
                bytesSent += await _socket.SendAsync(new ArraySegment<byte>(_gameView.Whiteboard.GetLayer(1)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending: {ex.Message}, disconnecting...");
                Disconnect();
                return;
            }
            Thread.Sleep(10);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_gameView != null) _gameView.Dispose();
        base.Dispose(disposing);
    }
}