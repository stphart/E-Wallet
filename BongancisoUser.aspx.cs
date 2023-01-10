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
    public partial class BongancisoUser : System.Web.UI.Page
    {
        String conndb = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                panel_personal.Visible = true;
                panel_security.Visible = false;
                panel_delete.Visible = false;
                txtLN.Enabled = false;
                txtFN.Enabled = false; 
                txtMN.Enabled = false;
                txtEA.Enabled = false;
                msg.Visible = false;
                msg2.Visible = false;
                msg3.Visible = false;
                btn_update.Enabled = false;
                btn_cancel.Visible = false;
                display_user_information();
            }
        }

        void display_user_information()
        {

            //display the user's information
            String username = Session["username"].ToString();
            using (var db = new SqlConnection(conndb))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT * FROM usersAcc WHERE username ='" + username + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        txtLN.Text = reader["lname"].ToString();
                        txtFN.Text = reader["fname"].ToString();
                        txtMN.Text = reader["mobilenum"].ToString();
                        txtEA.Text = reader["datebirth"].ToString();
                    }
                    
                }
                db.Close();
            }
        }


        protected void btn_edit_Click(object sender, EventArgs e)
        {
            //enable user to edit one's infomation
            btn_edit.Visible = false;
            btn_cancel.Visible = true;
            txtLN.Enabled = true;
            txtFN.Enabled = true;
            txtMN.Enabled = true;
            txtEA.Enabled = true;
            btn_update.Enabled = true;
        }

        protected void btn_update_Click(object sender, EventArgs e)
        {
            try { 
            //update the user's information on the database
            //using the newly inputted information
            String username = Session["username"].ToString();
            String getLN = txtLN.Text;
            String getFN = txtFN.Text;
            String getMN = txtMN.Text;
            String getEA = txtEA.Text;

            using (var db = new SqlConnection(conndb))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    //update the current balance
                    query.CommandType = CommandType.Text;
                    query.CommandText = "UPDATE usersAcc set lname = '" + getLN + "', " +
                        "fname = '"+getFN+"' ," +
                        "mobilenum = '" + getMN + "' ," +
                        "datebirth = '" + getEA + "' " +
                        "WHERE username = '" + username + "'";
                    var check = query.ExecuteNonQuery();

                    if(check > 0)
                    {
                        msg.Visible = true;
                        msg.Text = "Profile successfully updated!";
                        msg.ForeColor = System.Drawing.Color.DarkGreen;
                        txtLN.Enabled = false;
                        txtFN.Enabled = false;
                        txtMN.Enabled = false;
                        txtEA.Enabled = false;
                            btn_edit.Enabled = true;
                            btn_cancel.Enabled = false;
                    }
                    else
                    {
                        msg.Visible = true;
                        msg.Text = "An error has occur. Try again!";
                        msg.ForeColor = System.Drawing.Color.Firebrick;
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

        protected void btn_user1_Click(object sender, EventArgs e)
        {
            panel_personal.Visible = true;
            panel_security.Visible = false;
            panel_delete.Visible = false;
            msg.Visible = false;
            txtLN.Enabled = false;
            txtFN.Enabled = false;
            txtMN.Enabled = false;
            txtEA.Enabled = false;
            btn_update.Enabled = false;
            btn_cancel.Visible = false;
            btn_user1.ForeColor = System.Drawing.Color.White;
            btn_user2.ForeColor = System.Drawing.ColorTranslator.FromHtml("#addcd8");
            btn_user3.ForeColor = System.Drawing.ColorTranslator.FromHtml("#addcd8");
        }

        protected void btn_user2_Click(object sender, EventArgs e)
        {
            msg2.Visible = false;
            panel_personal.Visible = false;
            panel_security.Visible = true;
            panel_delete.Visible = false;
            currentpass.Text = "";
            txtP1.Text = "";
            txtP2.Text = "";
            btn_user2.ForeColor = System.Drawing.Color.White;
            btn_user1.ForeColor = System.Drawing.ColorTranslator.FromHtml("#addcd8");
            btn_user3.ForeColor = System.Drawing.ColorTranslator.FromHtml("#addcd8");
        }

        protected void btn_user3_Click(object sender, EventArgs e)
        {
            panel_delete.Visible = true;
            panel_personal.Visible=false;
            panel_security.Visible=false;
            msg3.Visible = false;
            passtxt.Text = "";
            btn_user3.ForeColor = System.Drawing.Color.White;
            btn_user1.ForeColor = System.Drawing.ColorTranslator.FromHtml("#addcd8");
            btn_user2.ForeColor = System.Drawing.ColorTranslator.FromHtml("#addcd8");
        }
        protected void btn_newpass_Click(object sender, EventArgs e)
        {
            
            if(check_current_password() == false)
            {
                msg2.Visible = true;
                msg2.Text = "Current password did not match!";
                msg2.ForeColor = System.Drawing.Color.Red;
                txtP1.Text = "";
                txtP2.Text = "";
            }
            else
            {
                update_new_password();
            }

            
        }

        bool check_current_password()
        {
            String username = Session["username"].ToString();
            String currentP = currentpass.Text;
            using (var db = new SqlConnection(conndb))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT password FROM usersAcc WHERE password ='" + currentP + "' AND username = '" +username +"'";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                
            }
        }

        void update_new_password()
        {
            try { 
            String username = Session["username"].ToString();
            String password = txtP2.Text;
            using (var db = new SqlConnection(conndb))
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
                        msg2.Visible = true;
                        msg2.Text = "Profile successfully updated!";
                        msg2.ForeColor = System.Drawing.Color.DarkGreen;
                        currentpass.Text = "";
                        txtP1.Text = "";
                        txtP2.Text = "";
                    }
                    else
                    {
                        msg2.Visible = true;
                        msg2.Text = "An error has occur. Try again!";
                        msg2.ForeColor = System.Drawing.Color.Firebrick;
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

        protected void btn_cont_Click(object sender, EventArgs e)
        {
            //check wallet balance
            //check password
            //delete the account and all transaction

            read();
            Double get = Convert.ToDouble(Session["getbalance"]);
            if (checkpassword() == false)
            {
                msg3.Visible = true;
                passtxt.Text = "";
            }
            else
            {
                if (get == 0)
                {
                    email_delete();
                    deleteacc();
                    deletetrans();
                    Response.Write("<script>alert('You're account has been successfully deleted')</script>");
                    Session.Abandon();
                    Response.Redirect("Default");
                    
                }
                else
                {
                    passtxt.Text = "";
                    Response.Write("<script>alert('You still have remaining balance in your wallet!')</script>");
                }
            }
        }



        void read()
        {
            
            try
            {
                

                //this function is to read the current balance of user
                String username = Session["username"].ToString();
                using (var db = new SqlConnection(conndb))
                {
                    db.Open();
                    using (var query = db.CreateCommand())
                    {
                        query.CommandType = CommandType.Text;
                        query.CommandText = "SELECT balance FROM usersAcc WHERE username ='" + username + "' ";
                        SqlDataReader reader = query.ExecuteReader();

                        if (reader.Read())
                        {
                            Session["getbalance"] = reader["balance"].ToString();
                            
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

        void email_delete()
        {

            var email = Session["email"].ToString();
            var username = Session["username"].ToString();
            String accnum = Session["accnum"].ToString();
            var name = Session["name"].ToString();
            var bodymsg = "Hi " + name + ", <br><br>" +
                "We are sad to hear with your latest action in the E-Wallet System.<br>" +
                "This message is to notify you that your account,<br>" + 
                "<h5>Username: "+ username + "</h5><br>"+
                "<h5>Account Number: "+ accnum + "</h5><br>" +
                "has been completely deleted from the database of E-wallet" +
                "We hope you are satisfied with our service even on a short notice." +
                "Best Regards, <br>" + "E-Wallet | B";


            MailMessage msg = new MailMessage("E-Wallet | Bonganciso <ewallet.shb@gmail.com>", email);
            msg.Subject = "Account Deleted";
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
            
        }

        bool checkpassword()
        {
            String username = Session["username"].ToString();
            String pass = passtxt.Text;
            String pass2 = "";
            using (var db = new SqlConnection(conndb))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT * FROM usersAcc WHERE username ='" + username + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        pass2 = reader["password"].ToString();

                    }

                    if (pass2 == pass)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }



                }
            }

        }

        void deleteacc()
        {
            String username = Session["username"].ToString();
            using (var db = new SqlConnection(conndb))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "Delete from usersAcc WHERE username ='" + username + "' ";
                    SqlDataReader reader = query.ExecuteReader();


                }
                db.Close();
            }
        }

        void deletetrans()
        {
            String username = Session["username"].ToString();
            using (var db = new SqlConnection(conndb))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "Delete from accTrans WHERE username ='" + username + "' ";
                    SqlDataReader reader = query.ExecuteReader();


                }
                db.Close();
            }
        }

        protected void btb_cancel_Click(object sender, EventArgs e)
        {
            passtxt.Text = "";
        }

        protected void btn_link1_Click (object sender, EventArgs e)
        {
            Response.Redirect("BongancisoDashboard");
        }
        protected void btn_link2_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoDeposit");
        }

        protected void btn_cancel_Click(object sender, EventArgs e)
        {
            display_user_information();
            msg.Visible = false;
            txtLN.Enabled = false;
            txtFN.Enabled = false;
            txtMN.Enabled = false;
            txtEA.Enabled = false;

            btn_edit.Visible = true;
            btn_cancel.Visible = false;
            btn_update.Enabled = false;
        }

        protected void btn_cancel2_Click(object sender, EventArgs e)
        {
            currentpass.Text = "";
            txtP1.Text = "";
            txtP2.Text = "";
        }
    }
}