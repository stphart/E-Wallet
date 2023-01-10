using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BongancisoE_wallet
{
    public partial class BongancisoSOA : System.Web.UI.Page
    {
        String connDB = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                readsoabalance();
                displayBalance.Text = Session["getsoa"].ToString();
                displayUser.Text = Session["name"].ToString();
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = false;
                panel5.Visible = false;
            }
                
            
        }

        protected void btn_soa_Click(object sender, EventArgs e)
        {
            try 
            { 
            int instruction = Convert.ToInt32(soaList.SelectedValue);

                switch(instruction)
                {
                    case 1:
                        display_all();
                        break;

                    case 2:
                        display_deposit();
                        break;

                    case 3:
                        display_withdraw();
                        break;

                    case 4:
                        display_sendmoney();
                        break;

                    case 5:
                        display_receivemoney();
                        break;

                    default:
                    break;
                }

            }catch(Exception ex)
            {
                Response.Write("<script>alert('Error!')</script>");
            }
        }

        void display_all()
        {
            panel1.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;
            string username = Session["username"].ToString();
            using(var db = new SqlConnection(connDB))
            {
                db.Open();
                SqlCommand cmd = new SqlCommand();
                String query = "Select trans_type, amount, sent_receiver, receive_sender, datetime from accTrans where username = '" + username + "'";
                cmd.CommandText = query;
                cmd.Connection = db;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                displayAll.DataSource = dt;
                displayAll.DataBind();
                panel1.Visible = true;
                db.Close();
            }
        }

        void display_deposit()
        {
            panel1.Visible = false;
            panel2.Visible = true;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;
            string username = Session["username"].ToString();
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                SqlCommand cmd = new SqlCommand();
                String query = "Select trans_type, amount, datetime from accTrans where username = '" + username + "' and trans_type = 'D' ";
                cmd.CommandText = query;
                cmd.Connection = db;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                displayDeposit.DataSource = dt;
                displayDeposit.DataBind();
                panel2.Visible = true;

                db.Close();
            }
        }

        void display_withdraw()
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel5.Visible = false;
            string username = Session["username"].ToString();
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                SqlCommand cmd = new SqlCommand();
                String query = "Select trans_type, amount, datetime from accTrans where username = '" + username + "' and trans_type = 'W' ";
                cmd.CommandText = query;
                cmd.Connection = db;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                displayWithdraw.DataSource = dt;
                displayWithdraw.DataBind();
                panel3.Visible = true;
                db.Close();
            }
        }

        void display_sendmoney()
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = true;
            panel5.Visible = false;
            string username = Session["username"].ToString();
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                SqlCommand cmd = new SqlCommand();
                String query = "Select trans_type, amount, sent_receiver, datetime from accTrans where username = '" + username + "' and trans_type = 'S' ";
                cmd.CommandText = query;
                cmd.Connection = db;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                displaySent.DataSource = dt;
                displaySent.DataBind();
                panel4.Visible = true;
                db.Close();
            }
        }

        void display_receivemoney()
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = true;
            string username = Session["username"].ToString();
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                SqlCommand cmd = new SqlCommand();
                String query = "Select trans_type, amount, receive_sender, datetime from accTrans where username = '" + username + "' and trans_type = 'R' ";
                cmd.CommandText = query;
                cmd.Connection = db;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                displayReceive.DataSource = dt;
                displayReceive.DataBind();
                panel5.Visible = true;
                db.Close();
            }
        }

        void readsoabalance()
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
                        Session["getsoa"] = reader["balance"].ToString();
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


        protected void btn_user2_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoWithdraw");
        }

        protected void btn_user3_Click(object sender, EventArgs e)
        {
            Response.Redirect("BongancisoSend");
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