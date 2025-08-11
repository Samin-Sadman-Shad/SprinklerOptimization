using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using SprinklerOptimization.Contracts;
using SprinklerOptimization.Models;

public class VisualizationService:IVisualizationService
{
    public void Generate2DVisualization(
        SprinklerLayoutResults result,
        List<SprinklerOptimization.Models.Point> roomCorners,
        List<Pipe> waterPipes,
        string outputFilePath = "SprinklerLayout2D.png")
    {
        var plotModel = new PlotModel { Title = "Sprinkler Layout Visualization" };
        plotModel.Background = OxyColors.White;

        // --- Room Boundary as PolygonAnnotation ---
        var roomPolygon = new PolygonAnnotation
        {
            Stroke = OxyColors.Black,
            StrokeThickness = 2,
            Fill = OxyColors.Undefined
        };
        foreach (var pt in roomCorners)
            roomPolygon.Points.Add(new DataPoint(pt.X, pt.Y));
        // Close the polygon by repeating the first point
        roomPolygon.Points.Add(new DataPoint(roomCorners[0].X, roomCorners[0].Y));
        plotModel.Annotations.Add(roomPolygon);

        // --- Water Pipes as LineSeries ---
        foreach (var pipe in waterPipes)
        {
            var lineSeries = new LineSeries
            {
                Color = OxyColors.Blue,
                StrokeThickness = 3,
                MarkerType = MarkerType.None,
            };

            lineSeries.Points.Add(new DataPoint(pipe.Start.X, pipe.Start.Y));
            lineSeries.Points.Add(new DataPoint(pipe.End.X, pipe.End.Y));

            plotModel.Series.Add(lineSeries);
        }

        // --- Sprinklers as ScatterSeries ---
        var sprinklers = new ScatterSeries
        {
            MarkerFill = OxyColors.Red,
            MarkerType = MarkerType.Circle,
            MarkerSize = 5
        };

        foreach (var sp in result.SprinklerPositions)
        {
            sprinklers.Points.Add(new ScatterPoint(sp.X, sp.Y));
        }
        plotModel.Series.Add(sprinklers);

        // --- Axes ---
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });

        // --- Export to PNG ---
        var pngExporter = new OxyPlot.SkiaSharp.PngExporter { Width = 800, Height = 600 };
        using var stream = File.OpenWrite(outputFilePath);
        pngExporter.Export(plotModel, stream);

        Console.WriteLine($"OxyPlot visualization saved to {outputFilePath}");
    }

/*    public void ShowPlot(
        SprinklerLayoutResults result,
        List<SprinklerOptimization.Models.Point> roomCorners,
        List<Pipe> waterPipes)
    {
        var plotModel = new PlotModel { Title = "Sprinkler Layout" };
        plotModel.Background = OxyColors.White;

        // Room boundary polygon
        var roomPolygon = new PolygonAnnotation
        {
            Stroke = OxyColors.Black,
            StrokeThickness = 2,
            Fill = OxyColors.Undefined
        };
        foreach (var pt in roomCorners)
            roomPolygon.Points.Add(new DataPoint(pt.X, pt.Y));
        roomPolygon.Points.Add(new DataPoint(roomCorners[0].X, roomCorners[0].Y)); // Close polygon
        plotModel.Annotations.Add(roomPolygon);

        // Water pipes as lines
        foreach (var pipe in waterPipes)
        {
            var lineSeries = new LineSeries
            {
                Color = OxyColors.Blue,
                StrokeThickness = 3,
                MarkerType = MarkerType.None,
            };
            lineSeries.Points.Add(new DataPoint(pipe.Start.X, pipe.Start.Y));
            lineSeries.Points.Add(new DataPoint(pipe.End.X, pipe.End.Y));
            plotModel.Series.Add(lineSeries);
        }

        // Sprinklers as scatter points
        var sprinklers = new ScatterSeries
        {
            MarkerFill = OxyColors.Red,
            MarkerType = MarkerType.Circle,
            MarkerSize = 5
        };
        foreach (var sp in result.SprinklerPositions)
        {
            sprinklers.Points.Add(new ScatterPoint(sp.X, sp.Y));
        }
        plotModel.Series.Add(sprinklers);

        // Axes
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });

        // Create and show the Windows Form with PlotView
        var form = new System.Windows.Forms.Form
        {
            Text = "Sprinkler Layout Visualization",
            Width = 800,
            Height = 600
        };

        var plotView = new PlotView
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            Model = plotModel
        };

        form.Controls.Add(plotView);

        // Run the form (this blocks until window closed)
        System.Windows.Forms.Application.Run(form);
    }*/
}
