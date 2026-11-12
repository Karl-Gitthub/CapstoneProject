using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CapstoneProject.Presentation;
using CapstoneProject.Services;

internal static class GradeContextTests
{
    private sealed class Workspace : SchoolWorkspace
    {
        public bool AllowDiscard;
        public int Prompts;
        public Workspace(UserSession user) : base(user) { }
        protected override bool ConfirmGradeDiscard() { Prompts++; return AllowDiscard; }
    }
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void Wait(Func<bool> ready)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        while (timer.Elapsed.TotalSeconds < 15) { Application.DoEvents(); if (ready()) return; System.Threading.Thread.Sleep(20); }
        throw new Exception("Grade context transition did not complete");
    }
    public static void Integration(Action<string, Action> check, SchoolService instructor)
    {
        using (var form = new Workspace(instructor.User) { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false })
        {
            form.Show(); form.OpenPage("Grade entry");
            DataGridView grid = null;
            Wait(() => { grid = form.Controls.Find("Class grades", true).OfType<DataGridView>().FirstOrDefault(); return grid != null && grid.DataSource is DataTable; });
            var choices = form.Controls.Find("Class selector", true).OfType<ComboBox>().Single();
            var loaded = choices.Items.Cast<object>().Single(item => item.ToString().Contains("IT202") && item.ToString().Contains("2A") && item.ToString().Contains("2026-2027"));
            var other = choices.Items.Cast<object>().First(item => item != loaded);
            var open = FindButton(form, "Open class");
            var save = FindButton(form, "Save grades");
            var print = FindButton(form, "Print / export saved grades");
            var context = form.Controls.Find("loadedGradeClass", true).OfType<Label>().Single();
            choices.SelectedItem = loaded; open.PerformClick(); Application.DoEvents();
            var table = (DataTable)grid.DataSource;
            int id = Convert.ToInt32(table.Rows[0]["course_id"]);
            string original = Convert.ToString(table.Rows[0]["final"]);
            check("grade selector change blocks editing saving and printing until opened", () =>
            {
                choices.SelectedItem = other; Application.DoEvents();
                Require(!grid.Enabled && !save.Enabled && !print.Enabled, "Pending class selection left actions enabled");
                Require(context.Text.Contains(loaded.ToString()) && context.Text.Contains("Selection changed"), "Loaded class was not identified");
                Require(ReferenceEquals(table, grid.DataSource), "Selector silently replaced the editing table");
                choices.SelectedItem = loaded;
                Require(grid.Enabled && save.Enabled && print.Enabled, "Returning to the loaded class did not restore actions");
            });
            EditFinal(grid, "79");
            check("declining a class switch retains unsaved grades and restores the selector", () =>
            {
                choices.SelectedItem = other; open.PerformClick(); Application.DoEvents();
                Require(form.Prompts == 1 && ReferenceEquals(choices.SelectedItem, loaded), "Discard rejection did not restore selection");
                Require(Convert.ToString(table.Rows[0]["final"]) == "79" && save.Enabled, "Unsaved grades were lost");
                Require(Convert.ToString(instructor.Grades().AsEnumerable().Single(r => Convert.ToInt32(r["course_id"]) == id)["final"]) == original, "Declining a switch wrote grades");
            });
            check("grade save action persists the loaded class and reloads its context", () =>
            {
                save.PerformClick();
                Wait(() => !ReferenceEquals(table, grid.DataSource) && grid.Enabled && save.Enabled);
                Require(Convert.ToString(instructor.Grades().AsEnumerable().Single(r => Convert.ToInt32(r["course_id"]) == id)["final"]) == "79", "Loaded grade did not save");
                Require(context.Text.Contains(loaded.ToString()) && !context.Text.Contains("Selection changed"), "Saved context is incorrect");
            });
            other = choices.Items.Cast<object>().Single(item => item.ToString() == other.ToString());
            EditFinal(grid, "77");
            check("accepting a class switch discards only unsaved edits", () =>
            {
                form.AllowDiscard = true; choices.SelectedItem = other; open.PerformClick(); Application.DoEvents();
                Require(form.Prompts == 2, "Expected a discard prompt after the saved class became editable; got " + form.Prompts);
                Require(context.Text.Contains(other.ToString()) && !context.Text.Contains("Selection changed"), "Accepted switch did not load the new context: " + context.Text);
                Require(Convert.ToString(instructor.Grades().AsEnumerable().Single(r => Convert.ToInt32(r["course_id"]) == id)["final"]) == "79", "Discard overwrote the saved grade");
            });
            form.Hide();
        }
    }
    private static void EditFinal(DataGridView grid, string value)
    {
        grid.CurrentCell = grid.Rows[0].Cells["final"]; grid.Focus();
        Require(grid.BeginEdit(true), "Grade cell did not enter edit mode");
        ((TextBox)grid.EditingControl).Text = value;
        Require(grid.EndEdit(), "Grade cell did not validate");
        grid.BindingContext[grid.DataSource].EndCurrentEdit(); Application.DoEvents();
    }
    private static Button FindButton(Control parent, string text)
    {
        foreach (Control child in parent.Controls)
        {
            if (child is Button button && button.Text == text) return button;
            var found = FindButton(child, text); if (found != null) return found;
        }
        return null;
    }
}
