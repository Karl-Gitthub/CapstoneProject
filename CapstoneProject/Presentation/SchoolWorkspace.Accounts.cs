using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapstoneProject.Services;

namespace CapstoneProject.Presentation
{
    public partial class SchoolWorkspace
    {
        private readonly AccountAdministration accounts = new AccountAdministration();
        private async Task AccountsPage()
        {
            var page = Page("Manage staff roles and faculty links. Changes invalidate older sessions.", Auto(), Fill(), Auto());
            var hint = Theme.Label("Staff can request access from Sign in. Review pending requests before activating an account.");
            hint.MaximumSize = new System.Drawing.Size(900, 0);
            page.Controls.Add(hint, 0, 0);
            var grid = Theme.Grid("Staff accounts");
            page.Controls.Add(grid, 0, 1);
            var bar = Bar();
            var access = Theme.Button("Manage selected account", true);
            var reset = Theme.Button("Set temporary password");
            bar.Controls.Add(access);
            bar.Controls.Add(reset);
            page.Controls.Add(bar, 0, 2);
            Func<Task> reload = () => LoadData(() => accounts.Accounts(user), data => Bind(grid, data, "login_user", "login_fullname", "login_usertype", "app_faculty_id", "app_disabled"));
            access.Click += async (s, e) =>
            {
                var row = Selected(grid);
                if (row == null)
                {
                    NeedSelection();
                    return;
                }
                int id = Convert.ToInt32(row["ID"]), version = Convert.ToInt32(row["app_version"]);
                string username = Value(row, "login_user");
                using (var dialog = new FormDialog("Manage staff access", username + " · Role and program access come from this account configuration.", v => accounts.SetAccess(user, id, version, v["role"], v["faculty"], v["state"] == "Disabled")))
                {
                    dialog.Options("role", "Role", AccountAdministration.Roles, Value(row, "login_usertype")).Field("faculty", "Faculty identifier (required for faculty)", Value(row, "app_faculty_id")).Options("state", "Account state", new[] { "Active", "Disabled" }, row["app_disabled"] != DBNull.Value && Convert.ToBoolean(row["app_disabled"]) ? "Disabled" : "Active");
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        await reload();
                }
            };
            reset.Click += async (s, e) =>
            {
                var row = Selected(grid);
                if (row == null)
                {
                    NeedSelection();
                    return;
                }
                int id = Convert.ToInt32(row["ID"]), version = Convert.ToInt32(row["app_version"]);
                using (var dialog = new FormDialog("Set temporary password", Value(row, "login_user") + " · Verify the staff member's identity before resetting access. They must change this password at next sign-in.", v =>
                {
                    if (v["password"] != v["confirm"])
                        throw new ArgumentException("The passwords must match.");
                    accounts.ResetPassword(user, id, version, v["password"]);
                }, "Set temporary password"))
                {
                    dialog.Field("password", "Temporary password (12–128 characters)", password: true).Field("confirm", "Confirm password", password: true);
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        await reload();
                }
            };
            await reload();
        }
        private async Task RequestsPage()
        {
            var page = Page("Verify the staff member's identity and role before approving account access.", Fill(), Auto());
            var grid = Theme.Grid("Pending staff access requests");
            page.Controls.Add(grid, 0, 0);
            var bar = Bar();
            var review = Theme.Button("Review selected request", true);
            bar.Controls.Add(review);
            page.Controls.Add(bar, 0, 1);
            Func<Task> reload = () => LoadData(() => accounts.Requests(user), data => { Bind(grid, data, "username_text", "full_name", "email_text", "department_text", "reason_text", "created_utc"); if (data.Rows.Count == 0) feedback.Text = "No access requests are pending."; });
            review.Click += async (s, e) =>
            {
                var row = Selected(grid);
                if (row == null)
                {
                    NeedSelection();
                    return;
                }
                int id = Convert.ToInt32(row["request_id"]);
                using (var dialog = new FormDialog("Review staff access", Value(row, "full_name") + " · " + Value(row, "email_text") + "\nRequest: " + Value(row, "reason_text"), v => accounts.Review(user, id, v["decision"] == "Approve", v["role"], v["faculty"], v["note"]), "Apply decision"))
                {
                    dialog.Options("decision", "Decision", new[] { "Approve", "Reject" }).Options("role", "Approved role", AccountAdministration.Roles).Field("faculty", "Faculty identifier (required for faculty)").Field("note", "Review note / rejection reason", multiline: true);
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        await reload();
                }
            };
            await reload();
        }
        private async Task AuditPage()
        {
            var page = Page("Recent local workflow changes. Times are recorded in UTC.", Fill());
            var grid = Theme.Grid("Audit history");
            page.Controls.Add(grid, 0, 0);
            await LoadData(() => service.Audit(), data => Bind(grid, data));
        }
        private async Task RecoveryPage()
        {
            var page = Page("Verify the staff member through the school's established process before resetting their password.", Fill(), Auto());
            var grid = Theme.Grid("Password assistance requests");
            page.Controls.Add(grid, 0, 0);
            var review = Theme.Button("Review selected request", true);
            page.Controls.Add(review, 0, 1);
            Func<Task> reload = () => LoadData(() => accounts.RecoveryRequests(user), data => Bind(grid, data, "username_text", "email_text", "created_utc"));
            review.Click += async (s, e) =>
            {
                var row = Selected(grid);
                if (row == null)
                {
                    NeedSelection();
                    return;
                }
                int id = Convert.ToInt32(row["request_id"]);
                using (var dialog = new FormDialog("Review password assistance", Value(row, "username_text") + " · " + Value(row, "email_text") + "\nContact details alone do not prove identity. Supply a temporary password through your approved staff-verification process.", v =>
                {
                    bool approve = v["decision"] == "Reset password";
                    if (approve && v["password"] != v["confirm"])
                        throw new ArgumentException("The temporary passwords must match.");
                    accounts.ResolveRecovery(user, id, approve, v["password"], v["note"]);
                }, "Resolve request"))
                {
                    dialog.Options("decision", "Decision", new[] { "Reject", "Reset password" }).Field("password", "Temporary password (when resetting)", password: true).Field("confirm", "Confirm temporary password", password: true).Field("note", "Identity verification or rejection note *", multiline: true);
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        await reload();
                }
            };
            await reload();
        }
    }
}
