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
    public partial class testing : System.Web.UI.Page
    {
        String connDB = WebConfigurationManager.ConnectionStrings["conndb"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                if(Session["username"] == null)
                {
                    Response.Redirect("Default");
                }
                else
                {
                    displayAcc.Text = Session["accnum"].ToString();
                    DisplayDateReg.Text = Session["date"].ToString();
                    readbalance();
                    displayBalance.Text = Session["dashboardbalance"].ToString();
                    displayUser.Text = Session["name"].ToString();
                    pnl_logout.Visible = false;
                }
            }
        }

        void readbalance()
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
                    query.CommandText = "SELECT balance, lname, fname FROM usersAcc WHERE username ='" + username + "' ";
                    SqlDataReader reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                            Session["dashboardbalance"] = reader["balance"].ToString();
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

        protected void btn_logout_Click(object sender, EventArgs e)
        {
            pnl_logout.Visible = true;
        }

        protected void btn_back_Click(object sender, EventArgs e)
        {
            if(Session["username"] == null)
            {
                Response.Redirect("Default");
            }
            else
            {
                pnl_logout.Visible = false;
            }
        }

        protected void btn_out_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("Default");
        }
    }
}