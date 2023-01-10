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
    public partial class BongancisoWIthdraw : System.Web.UI.Page
    {
        String connDB = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                msg.Visible = false;

                readwithdraw();
                displayBalance.Text = Session["getwithdraw"].ToString();
                displayUser.Text = Session["name"].ToString();
            }
                
        }

        protected void btn_withdraw_Click(object sender, EventArgs e)
        {

            try
            {
                double getAmount = Convert.ToDouble(amt_withdraw.Text);
                String username = Session["username"].ToString();

                double checkAcc = Convert.ToDouble(Session["getwithdraw"]);

                if (checkAcc < getAmount)
                {
                    msg.Visible = true;
                    msg.Text = "You do not have sufficient balance to continue this transaction!";
                    msg.ForeColor = System.Drawing.Color.Firebrick;
                    amt_withdraw.Text = "";
                }
                else
                {
                    using (var db = new SqlConnection(connDB))
                    {
                        db.Open();
                        using (var query = db.CreateCommand())
                        {
                            query.CommandType = CommandType.Text;
                            query.CommandText = "INSERT INTO accTrans (username, trans_type, amount, datetime) "
                                                 + " VALUES (@username, @type, @amount, @date)";

                            query.Parameters.AddWithValue("@username", username);
                            query.Parameters.AddWithValue("@type", "Cash Out");
                            query.Parameters.AddWithValue("@amount", getAmount);
                            query.Parameters.AddWithValue("@date", DateTime.Now);

                            var ctr = query.ExecuteNonQuery();
                            if (ctr > 0)
                            {
                                updatewithdraw();
                                email_withdraw();
                                amt_withdraw.Text = "";
                                readwithdraw();
                                displayBalance.Text = Session["getwithdraw"].ToString();
                                msg.Visible = true;
                                msg.Text = "Successfully Cash Out ₱" + getAmount + " from your wallet.";
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
            }
            catch (Exception ex)
            {
                msg.Visible = true;
                msg.Text = "There is an error with your transaction. Try again!";
                msg.ForeColor = System.Drawing.Color.Firebrick;
            }   
        }

        void email_withdraw()
        {
            var email = Session["email"].ToString();
            var amount = amt_withdraw.Text;
            var sendername = Session["name"].ToString();
            
            var bodymsg = "Hi " + sendername + ", <br><br>" +
                "<h4>You cash out ₱" + amount + "</h4><br>" +
                "The amount has been deducted to your wallet balance. <br> <br>" +
                "Thanks, <br>" + "E-Wallet | B";


            MailMessage msg = new MailMessage("E-Wallet | Bonganciso <ewallet.shb@gmail.com>", email);
            msg.Subject = "Cash Out Successful";
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

        void updatewithdraw()
        {
            //this function is to subtract inputted withdraw value
            //to the current balance amoount the user have
            String username = Session["username"].ToString();
            readwithdraw();
            double getbalance = Convert.ToDouble(Session["getwithdraw"]);
            double getwithdraw = Convert.ToDouble(amt_withdraw.Text);
            double newbalance = getbalance - getwithdraw;
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "UPDATE usersAcc set balance = '" + newbalance + "' where username = '" + username + "'";
                    query.ExecuteNonQuery();

                }
                db.Close();
            }
        }


        void readwithdraw()
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
                        Session["getwithdraw"] = reader["balance"].ToString();
                        Session["name"] = reader["fname"].ToString() + " " + reader["lname"].ToString();
                    }

                }
                db.Close();
            }
        }

        protected void btn_user1_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoDeposit");
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