/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using CombatTable.Models.Custom;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;
using CombatTable.Models;

namespace CombatTable.UserControls
{
    public enum MapOperations
    {
        None =0,

        Wall,
        Door,
        PointOfInterest,
        Block,

        AddEffect,
        RemoveEffect
    }


    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl, INotifyPropertyChanged
    {
        private int focusedX = -1;
        private int focusedY = -1;
        private MapOperations editorOperation;
        private string editorParameter;
        private Models.Character focusedCharacter;

        public Map()
        {
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(MapChanged);
            InitializeComponent();
        }

        #region Properties

        public Models.Character FocusedCharacter
        {
            get { return focusedCharacter; }
            set
            {
                if (value != focusedCharacter)
                {
                    focusedCharacter = value;
                    Trigger("FocusedCharacter");
                }
            }
        }

        public string EditorParameter
        {
            get { return editorParameter; }
            set
            {
                if (value != editorParameter)
                {
                    editorParameter = value;
                    Trigger("EditorParameter");
                }
            }
        }

        public MapOperations EditorOperation
        {
            get { return editorOperation; }
            set
            {
                if (value != editorOperation)
                {
                    editorOperation = value;
                    Trigger("EditorOperation");
                }
            }
        }

        public Models.Custom.Map MapInfo
        {
            get { return DataContext as Models.Custom.Map; }
            set
            {
                if (value != DataContext)
                {
                    DataContext = value;
                    Trigger("MapInfo");
                }
            }
        }

        #endregion

        #region Editor Methods

        private void ConsumeEditorOperation(int pX, int pY)
        {
            int delta = CellSize / 6;

            int lineH = pY / CellSize, lineH2 = lineH + 1;
            int lineV = pX / CellSize, lineV2 = lineV + 1;

            int deltaH = pX % CellSize;
            int deltaV = pY % CellSize;

            //: new delta means actually it is closer to other side and acceptable distance only
            int newDeltaH = ((lineV + 1) * CellSize) - pX;
            if (newDeltaH < deltaH && newDeltaH < delta)
            {
                deltaH = newDeltaH;
                lineV = lineV2;
            }

            //: new delta means actually it is closer to other side and acceptable distance only
            int newDeltaV = ((lineH + 1) * CellSize) - pY;
            if (newDeltaV < deltaV && newDeltaV < delta)
            {
                deltaV = newDeltaV;
                lineH = lineH2;
            }

            //: too close to actual corner point, then ignore this
            if (deltaH < delta && deltaV < delta) return;

            if (EditorOperation == MapOperations.Door || EditorOperation == MapOperations.Wall)
            {
                //: point too in the middle, cannot determine the actual line, ignore this for AddWall/AddDoor
                if (deltaH > delta && deltaV > delta) return;

                Point first, second;

                //: horizontal line
                if (deltaH > deltaV)
                {
                    first = new Point(lineV, lineH);
                    second = new Point(lineV2, lineH);
                }
                //: vertical line
                else
                {
                    first = new Point(lineV, lineH);
                    second = new Point(lineV, lineH2);
                }

                //: special controls, not needed
                //if (Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y) > 1) throw new Exception("!");
                //else if (first == second) throw new Exception("!2");

                if (EditorOperation == MapOperations.Door)
                {
                    Models.Custom.Door existingDoor = MapInfo.Doors.FirstOrDefault(d => (d.First == first && d.Second == second) || (d.Second == first && d.First == second));
                    if (existingDoor != null) MapInfo.Doors.Remove(existingDoor);
                    else MapInfo.Doors.Add(new Models.Custom.Door { First = first, Second = second, IsOpen = false });
                }
                else
                {
                    Models.Custom.Wall existingWall = MapInfo.Walls.FirstOrDefault(d => (d.First == first && d.Second == second) || (d.Second == first && d.First == second));
                    if (existingWall != null) MapInfo.Walls.Remove(existingWall);
                    else MapInfo.Walls.Add(new Models.Custom.Wall { First = first, Second = second });
                }
            }
            else if (EditorOperation == MapOperations.PointOfInterest || EditorOperation == MapOperations.Block)
            {
                //: point too close to edges, calculation might be problem, ignore this
                if (deltaH < delta || deltaV < delta) return;

                //: no note? forgotten? ignore this
                if (string.IsNullOrEmpty(EditorParameter)) return;

                Point location = new Point(lineV, lineH);
                if (EditorOperation == MapOperations.PointOfInterest)
                {
                    Models.Custom.PointOfInterest poi = MapInfo.PointOfInterests.FirstOrDefault(p => p.Location == location);
                    if (poi != null) MapInfo.PointOfInterests.Remove(poi);
                    else MapInfo.PointOfInterests.Add(new Models.Custom.PointOfInterest { Location = location, Notes = EditorParameter });
                }
                else
                {
                    Models.Custom.Block block = MapInfo.Blocks.FirstOrDefault(p => p.Location == location);
                    if (block != null) MapInfo.Blocks.Remove(block);
                    else MapInfo.Blocks.Add(new Models.Custom.Block{ Location = location, Notes = EditorParameter });
                }
            }
            else if (EditorOperation == MapOperations.AddEffect)
            {
                //: point too close to edges, calculation might be problem, ignore this
                if (deltaH < delta || deltaV < delta) return;

                //: no note? forgotten? ignore this
                if (string.IsNullOrEmpty(EditorParameter)) return;

                Point location = new Point(lineV, lineH);
                Models.Custom.Effect eff = MapInfo.Effects.FirstOrDefault(p => p.Location == location);
                if (eff == null) MapInfo.Effects.Add(new Models.Custom.Effect{ Location = location, Notes = EditorParameter });
            }
            else if (EditorOperation == MapOperations.RemoveEffect)
            {
                //: point too close to edges, calculation might be problem, ignore this
                if (deltaH < delta || deltaV < delta) return;

                //: no note? forgotten? ignore this
                if (string.IsNullOrEmpty(EditorParameter)) return;

                Point location = new Point(lineV, lineH);
                Models.Custom.Effect eff = MapInfo.Effects.FirstOrDefault(p => p.Location == location);
                if (eff != null) MapInfo.Effects.Remove(eff);
                //else MapInfo.PointOfInterests.Add(new Models.Custom.PointOfInterest { Location = location, Notes = EditorParameter });
            }
        }

        #endregion

        #region Map Change Triggers
        private void MapChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Trigger("MapInfo");
            if (e.OldValue is Models.Custom.Map)
                UnRegisterFromMap(e.OldValue as Models.Custom.Map);
            if (e.NewValue is Models.Custom.Map)
                RegisterToMap(e.NewValue as Models.Custom.Map);

            ReDrawMap();
        }

        private void UnRegisterFromMap(Models.Custom.Map mapInfo)
        {
            if (mapInfo == null) return;

            mapInfo.Doors.CollectionChanged -= MapDoorCollectionChanged;
            mapInfo.Walls.CollectionChanged -= MapWallCollectionChanged;
            mapInfo.PointOfInterests.CollectionChanged -= MapPointOfInterestCollectionChanged;
            mapInfo.Characters.CollectionChanged -= MapCharacterCollectionChanged;
            mapInfo.Blocks.CollectionChanged -= MapBlockCollectionChanged;
            mapInfo.Effects.CollectionChanged -= MapEffectCollectionChanged;

            MAP.Children.Clear();
        }

        private void RegisterToMap(Models.Custom.Map mapInfo)
        {
            if (mapInfo == null) return;

            mapInfo.Doors.CollectionChanged += MapDoorCollectionChanged;
            mapInfo.Walls.CollectionChanged += MapWallCollectionChanged;
            mapInfo.PointOfInterests.CollectionChanged += MapPointOfInterestCollectionChanged;
            mapInfo.Characters.CollectionChanged += MapCharacterCollectionChanged;
            mapInfo.Blocks.CollectionChanged += MapBlockCollectionChanged;
            mapInfo.Effects.CollectionChanged += MapEffectCollectionChanged;
        }

        private void MapCollectionChanged<UC, MC>(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
            where UC : UserControl, ICellItem, new()
            where MC : Notifier, ICellModel
        {
            UC c;

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        List<MC> newItemsCasted = new List<MC>();
                        if (newItems != null)
                            foreach (var v in newItems)
                                if (v is MC) newItemsCasted.Add(v as MC);

                        foreach (var v in newItemsCasted)
                        {
                            c = new UC() { DataContext = v };
                            Canvas.SetLeft(c, v.Location.X * CellSize);
                            Canvas.SetTop(c, v.Location.Y * CellSize);
                            //Panel.SetZIndex(c, 12);
                            c.CellSizeChanged(CellSize);
                            MAP.Children.Add(c);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var arr = new UIElement[MAP.Children.Count];
                        MAP.Children.CopyTo(arr, 0);
                        UC[] objectList = arr.Where(d => d is UC).Cast<UC>().ToArray();
                        foreach (var v in oldItems)
                        {
                            MC h = (MC)v;
                            c = objectList.FirstOrDefault(f => f.DataContext == h);
                            if (c != null) MAP.Children.Remove(c);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void MapEffectCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MapCollectionChanged<UserControls.Effect, Models.Custom.Effect>(e.Action, e.NewItems, e.OldItems);
        }

        private void MapCharacterCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if( !EditorMode )
                MapCollectionChanged<UserControls.Character, Models.Character>(e.Action, e.NewItems, e.OldItems);
        }

        private void MapBlockCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MapCollectionChanged<UserControls.Block, Models.Custom.Block>(e.Action, e.NewItems, e.OldItems);
        }

        private void MapPointOfInterestCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MapCollectionChanged<UserControls.PointOfInterest, Models.Custom.PointOfInterest>(e.Action, e.NewItems, e.OldItems);
        }

        private void MapWallCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MapCollectionChanged<UserControls.Wall, Models.Custom.Wall>(e.Action, e.NewItems, e.OldItems);
        }

        private void MapDoorCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MapCollectionChanged<UserControls.Door, Models.Custom.Door>(e.Action, e.NewItems, e.OldItems);
        }
        #endregion

        #region Mouse Events
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || e.ClickCount != 1) return;

            Point p = Mouse.GetPosition(MAP);

            if (EditorOperation == MapOperations.AddEffect || EditorOperation == MapOperations.RemoveEffect)
                ConsumeEditorOperation(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));
            else if (!EditorMode)
            {
                int x = (int)(p.X) / CellSize;
                int y = (int)(p.Y) / CellSize;

                //: focused on a character
                Models.Character chr = MapInfo.Characters.FirstOrDefault(c => c.Session.CurrentCoordinate_X.IntegerValue == x && c.Session.CurrentCoordinate_Y.IntegerValue == y);
                if (chr != null)
                {
                    FocusedCharacter = chr;
                }
                //: move operation occured
                else if (FocusedCharacter != null)
                {
                    UIElement[] arr = new UIElement[MAP.Children.Count];
                    MAP.Children.CopyTo(arr, 0);

                    UserControls.Character characterUC = (UserControls.Character)arr.FirstOrDefault(f => f is UserControls.Character && (f as UserControls.Character).DataContext == FocusedCharacter);
                    Canvas.SetLeft(characterUC, CellSize * x);
                    Canvas.SetTop(characterUC, CellSize * y);
                    FocusedCharacter.Session.CurrentCoordinate_X.IntegerValue = x;
                    FocusedCharacter.Session.CurrentCoordinate_Y.IntegerValue = y;

                    //: way of setting handled
                    FocusedCharacter = null;
                }

                //if (x == focusedX && y == focusedY)
                //{
                //    focused.Visibility = Visibility.Collapsed;
                //    focusedX = focusedY = -1;
                //    return;
                //}

                //if (focused.Visibility == Visibility.Collapsed) focused.Visibility = Visibility.Visible;

                //Canvas.SetLeft(focused, x * CellSize);
                //Canvas.SetTop(focused, y * CellSize);

                //focusedX = x;
                //focusedY = y;
            }
            else ConsumeEditorOperation(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));
        }

        private void OnCellSizeChange(object sender, MouseWheelEventArgs e)
        {
            int k = e.Delta;

            if (k > 0 && CellSize < 128) CellSize += 4;
            else if (k < 0 && CellSize > 12) CellSize -= 4;
            focused.Width = CellSize - 2;
            focused.Height = CellSize - 2;
        }
        #endregion

        #region Map Draw Methods

        private void RemoveMapLinesAndTexts()
        {
            //: remove lines
            var list = new List<Line>(MAP.Children.Cast<UIElement>().
                Where(l => l is Line).
                Cast<Line>());
            foreach (var v in list) MAP.Children.Remove(v);
            //: remove location texts
            var list2 = new List<TextBlock>(MAP.Children.Cast<UIElement>().
                Where(l => l is TextBlock
                && (l as TextBlock).DataContext is string
                && Convert.ToString((l as TextBlock).DataContext).StartsWith("LBL")).
                Cast<TextBlock>());
            foreach (var v in list2) MAP.Children.Remove(v);
        }

        private void MoveObjectsOnMap<UC, MC>()
            where UC : UserControl, ICellItem
            where MC : ICellModel
        {
            var list = new List<UC>(MAP.Children.Cast<UIElement>()
                .Where(l => l is UC)
                .Cast<UC>());
            if (list.Count > 0)
            {
                foreach (var v in list)
                {
                    MC mc = (MC)v.DataContext;
                    if (mc != null)
                    {
                        Canvas.SetLeft(v, mc.Location.X * CellSize);
                        Canvas.SetTop(v, mc.Location.Y * CellSize);
                    }
                    v.CellSizeChanged(CellSize);
                }
            }
        }

        private void DrawObjectsOnMap<UC, MC>(ObservableCollection<MC> collection, int zindex)
            where UC : UserControl, ICellItem, new()
            where MC : ICellModel
        {
            foreach (var mc in collection)
            {
                UC uc = new UC { DataContext = mc };
                Canvas.SetLeft(uc, mc.Location.X * CellSize);
                Canvas.SetTop(uc, mc.Location.Y * CellSize);
                //Canvas.SetZIndex(uc, zindex);
                uc.CellSizeChanged(CellSize);

                MAP.Children.Add(uc);
            }
        }

        private void ReDrawMap()
        {
            //: remove existing map lines
            RemoveMapLinesAndTexts();

            //: map area size changes
            MAP.Width = CellSize * XSize;
            MAP.Height = CellSize * YSize;

            //: rows
            for (int i = 1; i < YSize; i++)
                MAP.Children.Add(new Line()
                {
                    X1 = 0,
                    X2 = MAP.Width,
                    Y1 = i * CellSize,
                    Y2 = i * CellSize,
                    Stroke = new SolidColorBrush() { Color = Colors.Black },
                    StrokeThickness = 1,
                    DataContext = string.Format("ROW.{0}", i)
                });

            //: columns
            for (int i = 1; i < XSize; i++)
                MAP.Children.Add(new Line
                {
                    X1 = i * CellSize,
                    X2 = i * CellSize,
                    Y1 = 0,
                    Y2 = MAP.Height,
                    Stroke = new SolidColorBrush { Color = Colors.Black },
                    StrokeThickness = 1,
                    DataContext = string.Format("COL.{0}", i)
                });

            //: row.column location texts
            for (int i = 0; i < YSize; i++)
                for (int j = 0; j < XSize; j++)
                {
                    var txb = new TextBlock
                    {
                        Foreground = new SolidColorBrush(Colors.Blue),
                        DataContext = string.Format("LBL.{0}.{1}", j, i)
                    };
                    Canvas.SetLeft(txb, j * CellSize);
                    Canvas.SetTop(txb, i * CellSize);
                    Panel.SetZIndex(txb, 10);
                    MAP.Children.Add(txb);

                    Binding bnd = new Binding("ShowLocations");
                    bnd.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
                    bnd.RelativeSource.AncestorType = typeof(UserControls.Map);
                    bnd.Converter = new BooleanToVisibilityConverter();
                    txb.SetBinding(TextBlock.VisibilityProperty, bnd);

                    txb.Inlines.Add(new Italic(new Run(string.Format("{0}.{1}", j, i))));
                }

            if (MapInfo != null)
            {
                //: draw effects
                DrawObjectsOnMap<UserControls.Effect, Models.Custom.Effect>(MapInfo.Effects, 10);

                //: draw characters
                if (!EditorMode)
                    DrawObjectsOnMap<UserControls.Character, Models.Character>(MapInfo.Characters, 40);

                //: draw infos
                DrawObjectsOnMap<UserControls.PointOfInterest, Models.Custom.PointOfInterest>(MapInfo.PointOfInterests, 30 );

                //: draw blocks
                DrawObjectsOnMap<UserControls.Block, Models.Custom.Block>(MapInfo.Blocks, 20 );

                //: draw doors
                DrawObjectsOnMap<UserControls.Door, Models.Custom.Door>(MapInfo.Doors, 15);

                //: draw walls
                DrawObjectsOnMap<UserControls.Wall, Models.Custom.Wall>(MapInfo.Walls, 15);
            }

            if (EditorMode)
            {
                focused.Visibility = Visibility.Collapsed;
                focusedX = focusedY = -1;
            }
            else
            {
                //: move focused cell also
                if (focusedX != -1 && focusedY != -1)
                {
                    Canvas.SetLeft(focused, focusedX * CellSize);
                    Canvas.SetTop(focused, focusedY * CellSize);
                }
            }
        }

        private void RedrawLayout()
        {
            //: map area size changes
            MAP.Width = CellSize * XSize;
            MAP.Height = CellSize * YSize;

            if (EditorMode)
            {
                focused.Visibility = Visibility.Collapsed;
                focusedX = focusedY = -1;
            }
            else
            {
                //: move focused cell also
                if (focusedX != -1 && focusedY != -1)
                {
                    Canvas.SetLeft(focused, focusedX * CellSize);
                    Canvas.SetTop(focused, focusedY * CellSize);
                }
            }

            List<Line> rows = new List<Line>(MAP.Children.Cast<UIElement>().Where(l => l is Line && Convert.ToString((l as Line).DataContext).StartsWith("ROW")).Cast<Line>());
            List<Line> cols = new List<Line>(MAP.Children.Cast<UIElement>().Where(l => l is Line && Convert.ToString((l as Line).DataContext).StartsWith("COL")).Cast<Line>());
            List<TextBlock> lbls = new List<TextBlock>(MAP.Children.Cast<UIElement>().Where(l => l is TextBlock && Convert.ToString((l as TextBlock).DataContext).StartsWith("LBL")).Cast<TextBlock>());

            //: move rows
            foreach (Line r in rows)
            {
                int i = Convert.ToInt32(r.DataContext.ToString().Replace("ROW.", ""));
                r.X2 = MAP.Width;
                r.Y1 = r.Y2 = i * CellSize;
            }

            //: move columns
            foreach (Line c in cols)
            {
                int j = Convert.ToInt32(c.DataContext.ToString().Replace("COL.", ""));
                c.Y2 = MAP.Height;
                c.X1 = c.X2 = j * CellSize;
            }

            //: move labels
            foreach (TextBlock t in lbls)
            {
                string[] coordinates = t.DataContext.ToString().Replace("LBL.", "").Split('.');
                int i = Convert.ToInt32(coordinates[0]);
                int j = Convert.ToInt32(coordinates[1]);

                Canvas.SetLeft(t, CellSize * i);
                Canvas.SetTop(t, CellSize * j);
            }

            //: move effects
            MoveObjectsOnMap<UserControls.Effect, Models.Custom.Effect>();

            //: move characters
            if( !EditorMode )
                MoveObjectsOnMap<UserControls.Character, Models.Character>();

            //: move infos
            MoveObjectsOnMap<UserControls.PointOfInterest, Models.Custom.PointOfInterest>();

            //: move doors
            MoveObjectsOnMap<UserControls.Door, Models.Custom.Door>();

            //: move walls
            MoveObjectsOnMap<UserControls.Wall, Models.Custom.Wall>();

            //: move blocks 
            MoveObjectsOnMap<UserControls.Block, Models.Custom.Block>();
        }
        #endregion

        #region Dependency Properties

        #region Dependency Property Definitions
        public int XSize
        {
            get { return (int)GetValue(XSizeProperty); }
            set { SetValue(XSizeProperty, value); }
        }
        public int YSize
        {
            get { return (int)GetValue(YSizeProperty); }
            set { SetValue(YSizeProperty, value); }
        }
        public int CellSize
        {
            get { return (int)GetValue(CellSizeProperty); }
            set { SetValue(CellSizeProperty, value); }
        }

        public bool ShowLocations
        {
            get { return (bool)GetValue(ShowLocationsProperty); }
            set { SetValue(ShowLocationsProperty, value); }
        }

        public bool EditorMode
        {
            get { return (bool)GetValue(EditorModeProperty); }
            set { SetValue(EditorModeProperty, value); }
        }
        #endregion

        #region Dependency Property Declerations
        public static readonly DependencyProperty ShowLocationsProperty = DependencyProperty.Register("ShowLocations", typeof(bool), typeof(Map), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange));
        public static readonly DependencyProperty XSizeProperty = DependencyProperty.Register("XSize", typeof(int), typeof(Map), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, MapSizeChanged));
        public static readonly DependencyProperty YSizeProperty = DependencyProperty.Register("YSize", typeof(int), typeof(Map), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, MapSizeChanged));
        public static readonly DependencyProperty CellSizeProperty = DependencyProperty.Register("CellSize", typeof(int), typeof(Map), new FrameworkPropertyMetadata(48, FrameworkPropertyMetadataOptions.Inherits, CellSizeChanged));
        public static readonly DependencyProperty EditorModeProperty = DependencyProperty.Register("EditorMode", typeof(bool), typeof(Map), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, EditorModeChanged));
        #endregion

        #region Dependency Property Change Events
        private static void MapSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Map mapUC = d as Map;
            if (mapUC != null)
            {
                mapUC.ReDrawMap();
                mapUC.OnTrigger("XSize");
                mapUC.OnTrigger("YSize");
            }
        }

        private static void EditorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Map mapUC = d as Map;
            if (mapUC != null)
            {
                mapUC.ReDrawMap();
                mapUC.OnTrigger("EditorMode");
            }
        }

        private static void CellSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Map mapUC = d as Map;
            if (mapUC != null)
            {
                mapUC.RedrawLayout();
                mapUC.OnTrigger("CellSize");
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Models.Map mapInfo = MapInfo;
            //mapInfo.Doors.Add(new Models.Door { First = new Point(3, 3), Second = new Point(3, 4) });
            //mapInfo.Walls.Add(new Models.Wall { First = new Point(2, 2), Second = new Point(2,1) });
            //mapInfo.Characters.Add(new Models.Character { Code = "ASD", MapX = 4, MapY = 3 });
            //mapInfo.PointOfInterests.Add(new Models.PointOfInterest { Location = new Point(1, 1), Notes = "some chest" });
        }

        #endregion


        #region INotifyPropertyChanged Members
        protected void TriggerAll()
        {
            OnTriggerAll();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        protected void Trigger(string name)
        {
            OnTrigger(name);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected virtual void OnTriggerAll()
        {
        }

        protected virtual void OnTrigger(string name)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
