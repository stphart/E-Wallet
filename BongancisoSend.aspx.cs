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
    public partial class BongancisoSend : System.Web.UI.Page
    {
        String connDB = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                msg.Visible = false;

                readsent();
                displayBalance.Text = Session["getsent"].ToString();
                displayUser.Text = Session["name"].ToString();
            }
                
        }

        protected void btn_send_Click(object sender, EventArgs e)
        {

            try
            {
                double getAmount = Convert.ToDouble(txtamount.Text);
                String getReceiver = txtreceiver.Text;
                String username = Session["username"].ToString();

                double checkacc = Convert.ToDouble(Session["getsent"]);
                
                bool checkR = checkReceiver();
                String rUser = Session["rUser"].ToString();
                if (checkacc < getAmount)
                {
                    msg.Visible = true;
                    msg.Text = "You do not have sufficient balance to continue this transaction!";
                    msg.ForeColor = System.Drawing.Color.Firebrick;
                    txtamount.Text = "";
                    txtreceiver.Text = "";
                }
                else if(rUser == username)
                {
                    msg.Visible = true;
                    msg.Text = "Invalid receiver's username or email";
                    msg.ForeColor = System.Drawing.Color.Firebrick;
                    txtamount.Text = "";
                    txtreceiver.Text = "";
                }
                else if (checkR == false)
                {
                    msg.Visible = true;
                    msg.Text = "The system could not find the account you enter. Please input the account's correct username!";
                    msg.ForeColor = System.Drawing.Color.Firebrick;
                    txtamount.Text = "";
                    txtreceiver.Text = "";
                }
                else
                {
                    using (var db = new SqlConnection(connDB))
                    {
                        db.Open();
                        using (var query = db.CreateCommand())
                        {
                            query.CommandType = CommandType.Text;
                            query.CommandText = "INSERT INTO accTrans (username, trans_type, amount, sent_receiver, datetime) "
                                                 + " VALUES (@username, @type, @amount, @receiver, @date)";

                            readreceiver();
                            query.Parameters.AddWithValue("@username", username);
                            query.Parameters.AddWithValue("@type", "Send Money");
                            query.Parameters.AddWithValue("@amount", getAmount);
                            query.Parameters.AddWithValue("@receiver", Session["receivername"].ToString());
                            query.Parameters.AddWithValue("@date", DateTime.Now);

                            var ctr = query.ExecuteNonQuery();
                            if (ctr > 0)
                            {
                                updatesent();
                                updatereceiver();
                                addReceiver();
                                email_transfer_receiver();
                                email_transfer_sender();
                                txtamount.Text = "";
                                txtreceiver.Text = "";
                                readsent();
                                
                                displayBalance.Text = Session["getsent"].ToString();
                                msg.Visible = true;
                                msg.Text = "Successfully sent ₱" + getAmount + " to " + getReceiver + " and deducted ₱" +getAmount+ " from your wallet";
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

        void email_transfer_receiver()
        {
            try
            {

                var email = Session["receiveremail"].ToString();
                var amount = txtamount.Text;
                var sendername = Session["name"].ToString();
                var receivername = Session["receivername"].ToString();
                var bodymsg = "Hi " + receivername + ", <br><br>" +
                    "<h4>You receive ₱" + amount + " from " + sendername + "</h4> <br>" +
                    "The amount has been added to your wallet balance. <br> <br>" +
                    "Thanks, <br>" + "E-Wallet | B";


                MailMessage msg = new MailMessage("E-Wallet | Bonganciso <ewallet.shb@gmail.com>", email);
                msg.Subject = "Money Transfer Receiver";
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

        void email_transfer_sender()
        {
            var email = Session["email"].ToString();
            var amount = txtamount.Text;
            var sendername = Session["name"].ToString();
            var receivername = Session["receivername"].ToString();
            var bodymsg = "Hi " + sendername + ", <br><br>" +
                "<h4>You sent ₱" + amount + " to " + receivername + "</h4> <br>" +
                "The amount has been deducted to your wallet balance. <br> <br>" +
                "Thanks, <br>" + "E-Wallet | B";


            MailMessage msg = new MailMessage("E-Wallet | Bonganciso <ewallet.shb@gmail.com>", email);
            msg.Subject = "Money Transfer Sender";
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

        void updatesent()
        {
            //this function is to subtract inputted sent value
            //to the current balance amount the user have
            String username = Session["username"].ToString();
            readsent();
            double getbalance = Convert.ToDouble(Session["getsent"]);
            double getdeposit = Convert.ToDouble(txtamount.Text);
            double newbalance = getbalance - getdeposit;
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

        void readsent()
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
                        Session["getsent"] = reader["balance"].ToString();
                        Session["name"] = reader["fname"].ToString() + " " + reader["lname"].ToString();
                    }

                }
                db.Close();
            }
        }

        void updatereceiver()
        {
            //this function is to update the balance of the receiver
            //the amount inputted by the user will be added to the balance of the receiver
            readreceiver();
            String getReceiver = Session["getreceiverusername"].ToString();
           
            double getbalance = Convert.ToDouble(Session["getreceiver"]);
            double getTransfer_receiver = Convert.ToDouble(txtamount.Text);
            double newbalance = getbalance + getTransfer_receiver;
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "UPDATE usersAcc set balance = '" + newbalance + "' WHERE username = '" + getReceiver + "' ";
                    query.ExecuteNonQuery();
                    
                    
                }
                db.Close();
            }
        }

        
    void readreceiver()
        {
            //read the balance of the person the user want to transfer the money to

            String getRusername = txtreceiver.Text;
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT username, balance, lname, fname, email FROM usersAcc WHERE username ='" + getRusername + "' OR email = '"+ getRusername+"'";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        Session["getreceiver"] = reader["balance"].ToString();
                        Session["getreceiverusername"] = reader["username"].ToString();
                        Session["receivername"] = reader["fname"].ToString() + " " + reader["lname"].ToString();
                        Session["receiveremail"] = reader["email"].ToString();
                        
                    }

                }
                db.Close();
            }
        }

        Boolean checkReceiver()
        {
            String getRusername = txtreceiver.Text;
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var query = db.CreateCommand())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = "SELECT * FROM usersAcc WHERE username ='" + getRusername + "' OR email ='" + getRusername + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        Session["rUser"] = reader["username"].ToString();
                        return true;

                    }
                    else
                    {
                        return false;
                        
                    }

                }
                
            }
        }

        void addReceiver()
        {
            String receiverUsername = Session["getreceiverusername"].ToString();
            Double getAmount = Convert.ToDouble(txtamount.Text);
            String sender = Session["name"].ToString();
            using (var dbb = new SqlConnection(connDB))
            {
                dbb.Open();
                using (var queryy = dbb.CreateCommand())
                {
                    queryy.CommandType = CommandType.Text;
                    queryy.CommandText = "INSERT INTO accTrans (username, trans_type, amount, receive_sender, datetime) "
                                         + " VALUES (@Rusername, @Rtype, @Ramount, @receiver, @Rdate)";


                    queryy.Parameters.AddWithValue("@Rusername", receiverUsername);
                    queryy.Parameters.AddWithValue("@Rtype", "Receive");
                    queryy.Parameters.AddWithValue("@Ramount", getAmount);
                    queryy.Parameters.AddWithValue("@receiver", sender);
                    queryy.Parameters.AddWithValue("@Rdate", DateTime.Now);

                    queryy.ExecuteNonQuery();
                    
                   
                }

                dbb.Close();

            }

        }

        protected void btn_user1_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoDeposit");
        }
        protected void btn_user2_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoWithdraw");
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