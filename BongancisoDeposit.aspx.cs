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
    public partial class BongancisoDeposit : System.Web.UI.Page
    {
        String connDB = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                readdeposit();
                displayBalance.Text = Session["getdeposit"].ToString();
                displayUser.Text = Session["name"].ToString();
                msg.Visible = false;
            }

            
            
        }

        protected void btn_deposit_Click(object sender, EventArgs e)
        {
            try
            {
                double getAmount = Convert.ToDouble(amt_deposit.Text);
                String username = Session["username"].ToString();
                using (var db = new SqlConnection(connDB))
                {
                    db.Open();
                    using (var query = db.CreateCommand())
                    {
                        //insert the deposit transaction to the accTrans table 
                        query.CommandType = CommandType.Text;
                        query.CommandText = "INSERT INTO accTrans (username, trans_type, amount, datetime) "
                                             + " VALUES (@username, @type, @amount, @date)";


                        query.Parameters.AddWithValue("@username", username);
                        query.Parameters.AddWithValue("@type", "Cash In");
                        query.Parameters.AddWithValue("@amount", getAmount);
                        query.Parameters.AddWithValue("@date", DateTime.Now);

                        var ctr = query.ExecuteNonQuery();

                        //if the insertion is successful, the balance of the user will also be updated
                        if (ctr > 0)
                        {
                            updatedeposit();
                            email_deposit();
                            amt_deposit.Text = "";
                            readdeposit();
                            displayBalance.Text = Session["getdeposit"].ToString();
                            msg.Visible=true;
                            msg.Text = "Successfully Cash In ₱" +getAmount+" to your wallet.";
                            msg.ForeColor = System.Drawing.Color.DarkGreen;
                        }
                        else
                        {
                            msg.Visible = true;
                            msg.Text = "There is an error with your transaction. Try again!";
                            msg.ForeColor = System.Drawing.Color.Firebrick;
                        }

                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                msg.Visible = true;
                msg.Text = "There is an error with your transaction. Try again!";
                msg.ForeColor = System.Drawing.Color.Firebrick;
            }
        }

        void email_deposit()
        {
            try
            {

                var email = Session["email"].ToString();
                var amount = amt_deposit.Text;
                var sendername = Session["name"].ToString();
            
                var bodymsg = "Hi " + sendername + ", <br><br>" +
                    "<h4>You cash in ₱" + amount + "</h4><br>"+
                    "The amount has been added to your wallet balance. <br> <br>" +
                    "Thanks, <br>" + "E-Wallet | B";


                MailMessage msg = new MailMessage("E-Wallet | Bonganciso <ewallet.shb@gmail.com>", email);
                msg.Subject = "Cash In Successful";
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
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error!')</script>");
            }
        }

        void updatedeposit()
        {
            try
            {

                //this function is to add inputted deposit value
                //to the current balance amount the user have
                String username = Session["username"].ToString();
                readdeposit();
                double getbalance = Convert.ToDouble(Session["getdeposit"]);
                double getdeposit = Convert.ToDouble(amt_deposit.Text);
                double newbalance = getbalance + getdeposit;
                using (var db = new SqlConnection(connDB))
                {
                    db.Open();
                    using (var query = db.CreateCommand())
                    {
                        //update the current balance
                        query.CommandType = CommandType.Text;
                        query.CommandText = "UPDATE usersAcc set balance = '" + newbalance + "' where username = '" + username + "'";
                        query.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }catch(Exception ex)
            {
                Response.Write("<script>alert('Error!')</script>");
            }
}

        void readdeposit()
        {
            try
            {

                //this function is to read the current balance of user
                String username = Session["username"].ToString();
                using (var db = new SqlConnection(connDB))
                {
                    db.Open();
                    using (var query = db.CreateCommand())
                    {
                        query.CommandType = CommandType.Text;
                        query.CommandText = "SELECT * FROM usersAcc WHERE username ='" + username + "' ";
                        SqlDataReader reader = query.ExecuteReader();

                        if (reader.Read())
                        {
                            Session["getdeposit"] = reader["balance"].ToString();
                            Session["name"] = reader["fname"].ToString() + " " + reader["lname"].ToString();
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

        protected void btn_user2_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoWithdraw");
        }

        protected void btn_user3_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoSend");
        }

        protected void btn_user4_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoSOA");
        }

        protected void btn_user5_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoReport");
        }

        protected void btn_link1_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoDashboard");
        }
        protected void btn_link2_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoUser");
        }

    }
}