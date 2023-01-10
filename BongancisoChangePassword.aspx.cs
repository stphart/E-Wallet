using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace BongancisoE_wallet
{
    public partial class WebForm5 : System.Web.UI.Page
    {
        String connDB = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                send_email();
                get_code.Visible = true;
                get_new_password.Visible = false;
                error_cp.Visible = false;

            }
        }

        int generate_code()
        {
            Random random = new Random();
            int number = random.Next(10000, 99999);
            Session["generate_num"] = number;
            return number;
            
        }

        void send_email()
        {
            int num = generate_code();
            var email = Session["cp_email"].ToString();
            var username = Session["cp_username"].ToString();
            String accnum = Session["cp_accnum"].ToString();
            var name = Session["cp_name"].ToString();
            var bodymsg = "Hi " + name + ", <br>Forgot your password? <br><br>We receive a request coming from your account: <br>" +
                "Username: " + username +
                "<br>Accout number: " + accnum +
                "<br><br>To confirm this action, use this code to reset your password " + "<h2>"+num+"</h2> <br><br>" +
                "If you did not make this request then kindly ignore this email. <br><br>" +
                "Thanks, <br>" + "E-Wallet | B";


            MailMessage msg = new MailMessage("E-Wallet | Bonganciso <ewallet.shb@gmail.com>", email);
            msg.Subject = "Reset Password Confirmation";
            msg.Body = bodymsg;
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential()
            {
                UserName = "ewallet.shb@gmail.com",
                Password = "epnakkmvtjnghpwq"
            };
            smtp.EnableSsl = true;
            smtp.Send(msg);
            sentcp.Visible = true;

        }

        protected void btn_resend_Click(object sender, EventArgs e)
        {
            send_email();
        }

        protected void btn_change_pass_Click(object sender, EventArgs e)
        {
            try { 
            var password = cp_pass2.Text;
            var username = Session["cp_username"].ToString();
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    //update the current balance
                    query.CommandType = CommandType.Text;
                    query.CommandText = "UPDATE usersAcc set password = '" + password + "' where username = '" + username + "'";
                    var check = query.ExecuteNonQuery();

                    if (check > 0)
                    {
                        Response.Write("<script>alert('Password has been successfully updated!')</script>");
                        Response.Redirect("BongancisoLogin");
                    }
                    else
                    {
                        Response.Write("<script>alert('Error password cannot be updated!')</script>");
                    }
                }
                db.Close();
            }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error!')</script>");
            }
        }

            protected void btn_verify_Click(object sender, EventArgs e)
        {
            var inputcode = Convert.ToInt32(input_code.Text);
            var code = Convert.ToInt32(Session["generate_num"]);

            if(inputcode != code)
            {
                error_cp.Visible = true;
            }    
            else
            {
                get_new_password.Visible = true;
                get_code.Visible = false;

            }
        }

        protected void input_code_TextChanged(object sender, EventArgs e)
        {
            sentcp.Visible = false;
        }
    }
}