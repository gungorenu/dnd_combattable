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
using System.Xml;
using Microsoft.Win32;
using CombatTable.Models;

namespace CombatTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CombatSimulator.RollEngine.ExecutionEngine RollerEngine = new CombatSimulator.RollEngine.ExecutionEngine();
        private LootGenerator LootGenerator;

        public MainWindow()
        {
            InitializeComponent();

            RollerEngine = new CombatSimulator.RollEngine.ExecutionEngine();
            LootGenerator = new LootGenerator(RollerEngine);
            LootGenerator.NextTierModifier = GradualTreasureBonus;

            RollerEngine.VerboseNotification = true;
            RollerEngine.VerboseRollEventHandler += (t) => DieResultVerbose += t + "\n";
            LootGenerator.VerboseLog += (t) => TreasureVerbose += t;
        }

        private SessionManager session = new SessionManager();
        public SessionManager Session
        {
            get { return session; }
        }

        public UserControls.MapOperations[] EditorOperations
        {
            get
            {
                return new UserControls.MapOperations[] { UserControls.MapOperations.Door, UserControls.MapOperations.PointOfInterest, UserControls.MapOperations.Wall, UserControls.MapOperations.Block, UserControls.MapOperations.AddEffect, UserControls.MapOperations.RemoveEffect };
            }
        }

        #region Event Handlers

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    TC.SelectedIndex = 0;
                    e.Handled = true;
                    break;
                case Key.F2:
                    TC.SelectedIndex = 1;
                    e.Handled = true;
                    break;
                case Key.F3:
                    TC.SelectedIndex = 2;
                    e.Handled = true;
                    break;
                case Key.F4:
                    TC.SelectedIndex = 3;
                    e.Handled = true;
                    break;
                case Key.F5:
                    TC.SelectedIndex = 4;
                    e.Handled = true;
                    break;
                case Key.F6:
                    TC.SelectedIndex = 5;
                    e.Handled = true;
                    break;
            }
        }

        #region Character Tab
        private void CreateNewCharacter(object sender, RoutedEventArgs e)
        {
            Models.Character chr = new Models.Character("<< New Character >>");
            chr.Value = "NEW";
            Session.Characters.Add(chr);
            Session.SelectedCharacter = chr;
        }

        private void DeleteSelectedCharacter(object sender, RoutedEventArgs e)
        {
            if (Session.SelectedCharacter == null) return;
            Session.Characters.Remove(Session.SelectedCharacter);
            if (Session.SelectedMap != null) Session.SelectedMap.Characters.Remove(Session.SelectedCharacter);
            if (COMBATMAP.FocusedCharacter == Session.SelectedCharacter)
                COMBATMAP.FocusedCharacter = null;
        }

        private void CloneSelectedCharacter(object sender, RoutedEventArgs e)
        {
            if (Session.SelectedCharacter == null) return;

            XmlDocument xdoc = Session.SelectedCharacter.Export();
            CombatTable.Models.Character newChar = CombatTable.Models.Character.ReadCharacterFromElement(xdoc.DocumentElement);
            int count = Session.Characters.Count((f) => f.BaseInfo.Value == newChar.BaseInfo.Value);
            newChar.BaseInfo.Value = newChar.BaseInfo.Value + "_" + (count + 1).ToString();
            newChar.BaseInfo.Name = newChar.BaseInfo.Name + "_" + (count + 1).ToString();
            Session.Characters.Add(newChar);
        }

        private void CopyCharacter(object sender, RoutedEventArgs e)
        {
            if (Session.SelectedCharacter == null) return;

            XmlDocument xdoc = Session.SelectedCharacter.Export();
            Clipboard.SetText(xdoc.OuterXml);
        }

        private void PasteCharacter(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = Clipboard.GetText();
                if (string.IsNullOrEmpty(text)) return;

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(text);
                CombatTable.Models.Character newChar = CombatTable.Models.Character.ReadCharacterFromElement(xdoc.DocumentElement);

                int count = Session.Characters.Count((f) => f.BaseInfo.Value == newChar.BaseInfo.Value);
                newChar.BaseInfo.Value = newChar.BaseInfo.Value + "_" + (count + 1).ToString();
                newChar.BaseInfo.Name = newChar.BaseInfo.Name + "_" + (count + 1).ToString();

                Session.Characters.Add(newChar);
                Session.SelectedCharacter = newChar;
            }
            catch { }
        }

        #endregion

        #region Session Tab
        private void LoadSessionSearch(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DefaultExt = "xml";
            ofd.Filter = "Xml File (*.xml)|*.xml";
            ofd.ShowReadOnly = false;
            ofd.Title = "Open session file...";
            ofd.Multiselect = false;
            bool? res = ofd.ShowDialog();
            if (res.HasValue && res.Value)
                SessionLoadFile.Text = ofd.FileName;
        }

        private void SaveSessionSearch(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = true;
            sfd.CreatePrompt = false;
            sfd.DefaultExt = "xml";
            sfd.Filter = "Xml File (*.xml)|*.xml";
            sfd.OverwritePrompt = false;
            sfd.Title = "Write session file...";
            bool? res = sfd.ShowDialog();
            if (res.HasValue && res.Value)
                SessionSaveFile.Text = sfd.FileName;
        }

        private void LoadSession(object sender, RoutedEventArgs e)
        {
            try
            {
                Session.ReadFromFile(SessionLoadFile.Text);
                MessageBox.Show("Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void SaveSession(object sender, RoutedEventArgs e)
        {
            try
            {
                Session.WriteToFile(SessionSaveFile.Text);
                MessageBox.Show("Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ImportCharacterSearch(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DefaultExt = "xml";
            ofd.Filter = "Xml File (*.xml)|*.xml";
            ofd.ShowReadOnly = false;
            ofd.Title = "Import character from file...";
            ofd.Multiselect = false;
            bool? res = ofd.ShowDialog();
            if (res.HasValue && res.Value)
                ImportCharacterFile.Text = ofd.FileName;
        }

        private void ImportCharacter(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(ImportCharacterFile.Text);
                CombatTable.Models.Character newChar = CombatTable.Models.Character.ReadCharacterFromElement(xdoc.DocumentElement);
                if (!string.IsNullOrEmpty(ImportCharacterNewName.Text))
                    newChar.Name = ImportCharacterNewName.Text;
                Session.Characters.Add(newChar);
                MessageBox.Show("Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ExportCharacterSearch(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = true;
            sfd.CreatePrompt = false;
            sfd.DefaultExt = "xml";
            sfd.Filter = "Xml File (*.xml)|*.xml";
            sfd.OverwritePrompt = false;
            sfd.Title = "Export character to file...";
            bool? res = sfd.ShowDialog();
            if (res.HasValue && res.Value)
                ExportCharacterFile.Text = sfd.FileName;
        }

        private void ExportCharacter(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Session.SelectedCharacter == null)
                {
                    MessageBox.Show("No character selected!");
                    return;
                }

                Session.SelectedCharacter.Export(ExportCharacterFile.Text);
                MessageBox.Show("Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        #endregion
        #region Treasure Generator
        private void RollTreasure(int cr, bool npc)
        {
            DieResultVerbose = string.Empty;
            Treasure = string.Empty;
            TreasureVerbose = string.Empty;

            LootGenerator.NextTierModifier = GradualTreasureBonusNUD.Value;

            List<Loot> allLoots = new List<Loot>();

            for (int i = 0; i < TreasureCRCountNUD.Value; i++)
                allLoots.AddRange(LootGenerator.RollTreasure(cr, npc, AlwaysSetFirstTierCheckbox.IsChecked.Value, OnlySpellcasterCheckbox.IsChecked.Value));
            
            // only gold, shall merge
            List<Loot> goldLoot = new List<Loot>(allLoots.Where((l) => l.Type == TreasureType.Gold).ToArray());
            int goldTotal = 0;
            goldTotal = goldLoot.Aggregate<Loot,int>(0, (s,l) => s + Convert.ToInt32(l.Description));

            // non-gold loots
            allLoots = new List<Loot>(allLoots.Where((l) => l.Type != TreasureType.Gold).ToArray());
            allLoots.Sort();
            allLoots.Add(new Loot() { Type = TreasureType.Gold, Description = goldTotal.ToString() });

            foreach( var loot in allLoots)
                Treasure += loot.ToString() + "\r\n";

            Clipboard.SetText(Treasure);
        }

        private void RollTreasure(object sender, RoutedEventArgs e)
        {
            RollTreasure(TreasureCRNUD.Value, IsNPCCheckbox.IsChecked.Value);
        }

        #endregion

        #region Roll Die Tab
        private void RollDie(object sender, RoutedEventArgs e)
        {
            DieResultVerbose = string.Empty;
            try
            {
                DieResult = RollerEngine.ExecuteExpression(Die).ToString();
                if (CopyToClipboard)
                    Clipboard.SetText(DieResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Syntax Error: " + ex.Message);
            }
        }

        private void RollTemplateDie(object sender, RoutedEventArgs e)
        {
            DieResultVerbose = string.Empty;
            try
            {
                Die = Convert.ToString((sender as Button).Tag);
                if (Die == "Buy")
                    Die = "100 + (5*(20-(d20+   0   )))";
                else if (Die == "Sell")
                    Die = "100 + (5*( (d20 +   0   )-20))";

                DieResult = RollerEngine.ExecuteExpression(Die).ToString();
                if (CopyToClipboard)
                    Clipboard.SetText(DieResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Syntax Error: " + ex.Message);
            }
        }

        #endregion

        #region Combat Table Tab

        private void KillSelected(object sender, RoutedEventArgs e)
        {
            // no character selected
            if (COMBATMAP.FocusedCharacter == null) return;
            // already dead
            if (COMBATMAP.FocusedCharacter.States.FindProperties<State>((s) => s.CustomState.BooleanValue && s.Name == PredefinedStates.Dead.ToString()).Count() > 0)
                return;

            // dead state
            var state = COMBATMAP.FocusedCharacter.States.NewState();
            state.Name = CombatTable.Models.PredefinedStates.Dead.ToString();
            state.Duration.IntegerValue = 0;

            // set initiative and lose focus
            COMBATMAP.FocusedCharacter.Session.CurrentInitiative.IntegerValue = -1;
            Session.SelectedMap.Characters.Remove(COMBATMAP.FocusedCharacter);
            COMBATMAP.FocusedCharacter = null;
        }

        private void NextRound(object sender, RoutedEventArgs e)
        {
            if (COMBATMAP.FocusedCharacter == null) return;
            Models.Character c = COMBATMAP.FocusedCharacter;

            foreach (State cs in c.States.Properties)
            {
                if (cs.Duration.IntegerValue < 0) continue;
                cs.Duration.IntegerValue = cs.Duration.IntegerValue - 1;
                if (cs.Duration.IntegerValue == 0)
                    c.States.Remove(cs);
            }

            Die = string.Empty;
            DieResult = string.Empty;
            DieResultVerbose = string.Empty;
        }

        private void SetCurrentCharacter(object sender, MouseButtonEventArgs e)
        {
            CombatTable.Models.Character chr = COMBATMAP.FocusedCharacter;
            if (chr == null) return;
            else if (chr.Session.CurrentPlayer.BooleanValue) return;
            else
            {
                foreach (CombatTable.Models.Character m in Session.SelectedMap.Characters) m.Session.CurrentPlayer.BooleanValue = false;
                chr.Session.CurrentPlayer.BooleanValue = true;
            }
        }

        private void RemoveCharacterFromMap(object sender, RoutedEventArgs e)
        {
            if (COMBATMAP.FocusedCharacter == null) return;
            Session.SelectedMap.Characters.Remove(COMBATMAP.FocusedCharacter);
            COMBATMAP.FocusedCharacter = null;
        }

        private void SendCharacterIntoMap(object sender, RoutedEventArgs e)
        {
            if (Session.SelectedCharacter != null && Session.SelectedMap != null)
            {
                if (Session.SelectedMap.Characters.Contains(Session.SelectedCharacter))
                    return;

                int x = 0;
                int y = 0;
                bool found = false;
                do
                {
                    y = -1;
                    do
                    {
                        y++;
                        if (Session.SelectedMap.PointOfInterests.Any(a => a.Location.X == x && a.Location.Y == y))
                            continue;
                        if (Session.SelectedMap.Characters.Any(a => a.Session.CurrentCoordinate_X.IntegerValue == x && a.Session.CurrentCoordinate_Y.IntegerValue == y))
                            continue;

                        found = true;
                        break;
                    }
                    while (y < Session.SelectedMap.SizeY);

                    if (found) break;

                    x++;
                }
                while (x < Session.SelectedMap.SizeX);

                Session.SelectedCharacter.Session.CurrentCoordinate_X.IntegerValue = x;
                Session.SelectedCharacter.Session.CurrentCoordinate_Y.IntegerValue = y;
                Session.SelectedCharacter.Session.CurrentHitPoints.IntegerValue = Session.SelectedCharacter.BaseInfo.HitPoints.IntegerValue;
                Session.SelectedCharacter.Session.CurrentInitiative.IntegerValue = -1;

                Session.SelectedMap.Characters.Add(Session.SelectedCharacter);
            }
        }

        private void SessionOperations(object sender, RoutedEventArgs e)
        {
            DieResultVerbose = string.Empty;
            ComboBoxItem cbi = ExecuteCMB.SelectedItem as ComboBoxItem;
            if (cbi == null) return;
            int action = Convert.ToInt32(cbi.Tag);
            switch (action)
            {
                case 0: // Rest Selected Character
                    if (COMBATMAP.FocusedCharacter == null) return;
                    RestCharacter(COMBATMAP.FocusedCharacter);
                    break;
                case 1: // Rest All
                    if (Session.SelectedMap == null) return;
                    foreach (CombatTable.Models.Character c in Session.SelectedMap.Characters)
                        RestCharacter(c);
                    break;
                case 2: // Remove All Characters
                    if (Session.SelectedMap == null) return;
                    Session.SelectedMap.Characters.Clear();
                    break;
                case 3: // Reroll Inititative
                    foreach( Character chr in Session.SelectedMap.Characters)
                        RerollInitiative(chr); 
                    break;
                case 4: // Cast Spell on Selected Targets
                    CastSpell();
                    break;
                case 5: // Next Round
                    NextRound(null, null);
                    break;
                case 6: // Kill Selected
                    KillSelected(null, null);
                    break;
                case 7: // Roll Stealth/Perception
                    RollStealthPerception();
                    break;
                case 8: // Clear Stealth/Perception rolls
                    RemoveStealthPerception();
                    break;
                case 9: // Re-Check Combat Abilities for ALL
                    break;
                case 10: // Reroll Inititative New Comers
                    if (Session.SelectedMap == null) return;
                    foreach (CombatTable.Models.Character c in Session.SelectedMap.Characters)
                        if (c.Session.CurrentInitiative.IntegerValue == -1)
                             RerollInitiative(c); 
                    break;
                case 11: // Refresh Selected Character
                    if (COMBATMAP.FocusedCharacter == null) return;
                    RestCharacter(COMBATMAP.FocusedCharacter);
                    COMBATMAP.FocusedCharacter.States.Properties.Clear();
                    RerollInitiative(COMBATMAP.FocusedCharacter);
                    break;
                case 12: // Clear Stealth
                    RemoveStealth();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Map Tab

        private void CreateNewMap(object sender, RoutedEventArgs e)
        {
            Models.Custom.Map newMap = new Models.Custom.Map { Name = "<<New Map>>", SizeX = Convert.ToInt32(NewMapXSize.Text), SizeY = Convert.ToInt32(NewMapYSize.Text) };
            Session.Maps.Add(newMap);
            Session.EditorMap = newMap;
        }

        private void DeleteExistingMap(object sender, RoutedEventArgs e)
        {
            if (Session.EditorMap == null) return;

            if (MessageBox.Show(this, "Are you sure?", "Warning!", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            Session.Maps.Remove(Session.EditorMap);
            if (Session.SelectedMap == Session.EditorMap) Session.SelectedMap = Session.Maps.FirstOrDefault();
            Session.EditorMap = Session.Maps.FirstOrDefault();
        }

        #endregion

        #endregion

        #region Die Related Dependency Properties

        public string Die
        {
            get { return (string)GetValue(DieProperty); }
            set { SetValue(DieProperty, value); }
        }
        public string DieResultVerbose
        {
            get { return (string)GetValue(DieResultVerboseProperty); }
            set { SetValue(DieResultVerboseProperty, value); }
        }
        public string DieResult
        {
            get { return (string)GetValue(DieResultProperty); }
            set { SetValue(DieResultProperty, value); }
        }
        public bool CopyToClipboard
        {
            get { return (bool)GetValue(CopyToClipboardProperty); }
            set { SetValue(CopyToClipboardProperty, value); }
        }

        public static readonly DependencyProperty DieProperty = DependencyProperty.Register("Die", typeof(string), typeof(MainWindow), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DieResultProperty = DependencyProperty.Register("DieResult", typeof(string), typeof(MainWindow), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DieResultVerboseProperty = DependencyProperty.Register("DieResultVerbose", typeof(string), typeof(MainWindow), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CopyToClipboardProperty = DependencyProperty.Register("CopyToClipboard", typeof(bool), typeof(MainWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region Treasure Roller
        public int GradualTreasureBonus
        {
            get { return (int)GetValue(GradualTreasureBonusProperty); }
            set
            {
                SetValue(GradualTreasureBonusProperty, value);
                LootGenerator.NextTierModifier = value;
            }
        }

        public static readonly DependencyProperty GradualTreasureBonusProperty = DependencyProperty.Register("GradualTreasureBonus", typeof(int), typeof(MainWindow), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public string Treasure
        {
            get { return (string)GetValue(TreasureProperty); }
            set { SetValue(TreasureProperty, value); }
        }
        public static readonly DependencyProperty TreasureProperty = DependencyProperty.Register("Treasure", typeof(string), typeof(MainWindow), new UIPropertyMetadata(string.Empty));

        public string TreasureVerbose
        {
            get { return (string)GetValue(TreasureVerboseProperty); }
            set { SetValue(TreasureVerboseProperty, value); }
        }

        public static readonly DependencyProperty TreasureVerboseProperty =
            DependencyProperty.Register("TreasureVerbose", typeof(string), typeof(MainWindow), new UIPropertyMetadata(string.Empty));

        #endregion

        #region Combat Table Functionality

        private void RestCharacter(Character chr)
        {
            chr.Session.CurrentHitPoints.Value = chr.BaseInfo.HitPoints.Value;
            foreach (SpellcastingClass c in chr.Spellcasting.FindProperties<SpellcastingClass>())
            {
                foreach (SpellLevel l in c.SpellLevels)
                {
                    foreach (Spell s in l.Spells)
                        s.Available = true;
                }
            }
        }

        private void CastSpell()
        {
            List<CombatTable.Models.Character> chrs = new List<CombatTable.Models.Character>();
            foreach (object o in ChrGRID.SelectedItems)
                chrs.Add(o as CombatTable.Models.Character);
            if (chrs.Count == 0) return;

            foreach (CombatTable.Models.Character c in chrs)
            {
                State s = c.States.NewState();
                s.Name = ExecuteParameter.Text;
            }
        }

        private void RerollInitiative(Character chr)
        {
            if (Session.SelectedMap == null) return;

            chr.Session.CurrentInitiative.IntegerValue = RollerEngine.ExecuteExpression("d20 + " + chr.VitalStatistics.Initiative.IntegerValue);
        }

        private void RollSkill(Character c, SkillTypes skill)
        {
            // has skill already
            if (c.States.FindProperties<State>(s => s.CustomState.BooleanValue && s.Name == skill.ToString()).Count() > 0)
                return;

            int skillBonus = 0;
            Skill skillRef = c.Skills.FindProperty<Skill>((s) => s.Name == skill.ToString());
            if (skillRef != null)
                skillBonus += skillRef.IntegerValue;
            int roll = RollerEngine.ExecuteExpression("d20 + " + skillBonus);

            State sss = c.States.NewState();
            sss.CustomState.BooleanValue = true;
            sss.Name = skill.ToString();
            sss.Duration.IntegerValue = -1;
            sss.Value = roll.ToString();
        }

        private void RollStealthPerception()
        {
            foreach (CombatTable.Models.Character c in Session.SelectedMap.Characters)
            {
                RollSkill(c, SkillTypes.Stealth);
                RollSkill(c, SkillTypes.Perception);
            }
        }

        private void RemoveSkill(Character chr, SkillTypes skill)
        {
            State st = chr.States.FindProperty<State>((p) => p.Name == skill.ToString() && p.CustomState.BooleanValue);
            if (st != null) ;
            chr.States.Remove(st);
        }

        private void RemoveStealth()
        {
            foreach (CombatTable.Models.Character c in Session.SelectedMap.Characters)
            {
                RemoveSkill(c, SkillTypes.Stealth);
            }
        }
        private void RemoveStealthPerception()
        {
            foreach (CombatTable.Models.Character c in Session.SelectedMap.Characters)
            {
                RemoveSkill(c, SkillTypes.Stealth);
                RemoveSkill(c, SkillTypes.Perception);
            }
        }

        #endregion

    }
}
