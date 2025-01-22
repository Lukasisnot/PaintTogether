using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace PaintTogether;

public class Whiteboard : DrawableGameComponent
{
    public readonly int Width;
    public readonly int Height;
    private Game1 _game;
    private Point _position;
    private Point _mousePosition;
    private Point _mouseCanvasPos;
    private Point _lastMouseCanvasPos = Point.Zero;
    private int _brushSize = 8;
    private Color _brushColor = Color.Black;
    private Color _backgroundColor = Color.White;
    private readonly List<Texture2D> _layers = new();
    
    public Whiteboard(int width, int height, Point position, Game1 game) : base(game)
    {
        _position = position;
        _game = game;
        Width = width;
        Height = height;
        _mousePosition = new(0, 0);
    }

    public override void Initialize()
    {
        base.Initialize();
        AddLayer();
    }

    public override void Update(GameTime gameTime)
    {
        // Texture.GetData(_currentColors);
        
        base.Update(gameTime);
        _mousePosition = Mouse.GetState().Position;
        _mouseCanvasPos = _mousePosition - _position;

        // Console.WriteLine($"canvas: {_position}, mouse: {_mousePosition}, mouse canvas: {_mouseCanvasPos}");

        if (_mousePosition.X >= _position.X && _mousePosition.X <= _position.X + Width &&
            _mousePosition.Y >= _position.Y && _mousePosition.Y <= _position.Y + Height)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                Paint();
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                Paint(true);
        }
        
        _lastMouseCanvasPos = _mouseCanvasPos;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        DrawLayers();
    }

    public void AddLayer()
    {
        _layers.Add(new Texture2D(_game.GraphicsDevice, Width, Height));
    }
    
    public void RemoveLayer(int index)
    {
        _layers.RemoveAt(index);
    }

    public void DrawToLayer(int index, byte[] buffer)
    {
        // _layers[index].SetData(buffer);
        
        byte[] currentColorBytes = new byte[Width * Height * 4];
        _layers[index].GetData(currentColorBytes);
        for (int i = 0; i < Width * Height * 4; i += 4)
        {
            if (buffer[i + 3] == 0) continue;
        
            for (int j = 0; j < 4; j++)
            {
                currentColorBytes[i + j] = buffer[i + j];
            }
        }
        _layers[index].SetData(currentColorBytes);
    }

    public byte[] GetLayer(int index)
    {
        byte[] buffer = new byte[Width * Height * 4];
        _layers[index].GetData(buffer);
        return buffer;
    }

    public void DrawLayers()
    {
        foreach (var layer in _layers)
        {
            Rectangle destRect = new((int)_position.X, (int)_position.Y, Width, Height);
            _game.SpriteBatch.Draw(layer, destRect, Color.White);
        }
    }

    public byte[] GetCombinedLayers(int startLayerIndex)
    {
        byte[] output = new byte[Width * Height * 4];

        for (int k = startLayerIndex; k < _layers.Count; k++)
        {
            byte[] layer = GetLayer(k);
            for (int i = 0; i < Width * Height * 4; i += 4)
            {
                if (layer[i + 3] == 0) continue;
            
                for (int j = 0; j < 4; j++)
                {
                    output[i + j] = layer[i + j];
                }
            }
        }
        
        return output;
    }
    
    private void Paint(bool erase = false)
    {
        byte[] buffer = new byte[Width * Height * 4];

// lerping between current and last frame mouse position for smooth lines without gaps
        float lerpRes = Vector2.Distance(_mouseCanvasPos.ToVector2(), _lastMouseCanvasPos.ToVector2());
        for (int i = 0; i < (int)lerpRes; i++)
        {
            Point lerpMousePos = Vector2.Lerp(_lastMouseCanvasPos.ToVector2(), _mouseCanvasPos.ToVector2(), i/lerpRes).ToPoint();
            for (int x = -_brushSize/2; x < _brushSize/2; x++)
            {
                for (int y = -_brushSize/2; y < _brushSize/2; y++)
                {
                    int pixel = Math.Max(Math.Min(lerpMousePos.Y - 1 + y, Height - 1), 0) * Width + Math.Max(Math.Min(lerpMousePos.X + x, Width - 1), 0);
                    buffer[pixel * 4] = erase ? _backgroundColor.R : _brushColor.R;  // Blue channel
                    buffer[pixel * 4 + 1] = erase ? _backgroundColor.G : _brushColor.G;  // Green channel
                    buffer[pixel * 4 + 2] = erase ? _backgroundColor.B : _brushColor.B;  // Red channel
                    buffer[pixel * 4 + 3] = erase ? _backgroundColor.A : _brushColor.A;  // Alpha channel
                }
            }
        }
        DrawToLayer(0, buffer);
    }
}