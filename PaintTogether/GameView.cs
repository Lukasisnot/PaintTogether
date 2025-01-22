using System;
using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.UI;

namespace PaintTogether;

public class GameView : DrawableGameComponent
{
    public Whiteboard Whiteboard { get; private set; }
    private readonly Game1 _game;
    private Desktop _desktop;
    private string _endPoint;

    public GameView(Game1 game, string endPoint) : base(game)
    {
        _game = game;
        _endPoint = endPoint;
    }

    public override void Initialize()
    {
        Whiteboard = new Whiteboard(640, 460, new(0, 20), _game);
        // Whiteboard = new Whiteboard(200, 100, new(0, 20), _game);
        _game.Components.Add(Whiteboard);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        
        MyraEnvironment.Game = _game;

// Base grid 1 X 5
        var baseGrid = new Grid
        {
            RowSpacing = 8,
            ColumnSpacing = 8
        };
        
        baseGrid.HorizontalAlignment = HorizontalAlignment.Center;
        
        baseGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
        baseGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        baseGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        baseGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        baseGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        baseGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        
// Endpoint Title
        var endPoint = new Label
        {
            Id = "endPoint",
            Text = _endPoint
        };
        endPoint.HorizontalAlignment = HorizontalAlignment.Center;
        baseGrid.Widgets.Add(endPoint);
        
// Add it to the desktop
        _desktop = new Desktop();
        _desktop.Root = baseGrid;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        if (_desktop != null) _desktop.Render();
    }
}