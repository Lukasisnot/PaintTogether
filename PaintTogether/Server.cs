using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaintTogether;

public class Server : DrawableGameComponent
{
    private readonly Game1 _game;
    private GameView _gameView;
    private List<Socket> _sockets = new();
    private string _ipLocal;
    private readonly Socket _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

    public Server(Game1 game) : base(game)
    {
        _game = game;
        _ipLocal = _game.GetLocalIPAddress();
        _ipLocal = "127.0.0.1";
        EndPoint endPoint = new IPEndPoint(IPAddress.Parse(_ipLocal), 80);
        try
        {
            _socket.Bind(endPoint);
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Error while binding socket: {ex.Message}");
        }
        _socket.Listen(10);
    }

    public override void Initialize()
    {
        base.Initialize();
        _gameView = new GameView(_game, $"Host: {_ipLocal}");
        _game.Components.Add(_gameView);
        Task.Run(AcceptConnections);
        Task.Run(SendAll);
        Task.Run(ReceiveAll);
    }

    private void AddClient(Socket socket)
    {
        _sockets.Add(socket);
        _gameView.Whiteboard.AddLayer();
    }
    
    private void RemoveClient(int index)
    {
        _sockets.RemoveAt(index);
        _gameView.Whiteboard.RemoveLayer(index);
    }

    private async Task AcceptConnections()
    {
        while (true)
        {
            Socket connection = await _socket.AcceptAsync();
            AddClient(connection);
        }
    }
    
    private async Task SendAll()
    {
        while (true)
        {
            for (int i = 0; i < _sockets.Count; i++)
            {
                var socket = _sockets[i];
                try
                {
                    int bytesSent = 0;
                    bytesSent += await socket.SendAsync(new ArraySegment<byte>(_gameView.Whiteboard.GetCombinedLayers(0)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while sending: {ex.Message}, removing client");
                    RemoveClient(i);
                }
            }
            Thread.Sleep(10);
        }
    }

    private async Task ReceiveAll()
    {
        while (true)
        {
            for (int i = 0; i < _sockets.Count; i++)
            {
                try
                {
                    byte[] clientBuffer = new byte[_gameView.Whiteboard.Width * _gameView.Whiteboard.Height * 4];
                    int bytesReceived = await _sockets[i].ReceiveAsync(clientBuffer);

                    _gameView.Whiteboard.DrawToLayer(i + 1, clientBuffer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while receiving: {ex.Message}, removing client");
                    RemoveClient(i);
                }
                Thread.Sleep(10);
            }
        }
    }
    
}