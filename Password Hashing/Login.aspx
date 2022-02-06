<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Password_Hashing.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <style>
        .mainDiv {
            display: flex;
            min-height: 100%;
            align-items: center;
            justify-content: center;
            background-color: #f9f9f9;
            font-family: 'Open Sans', sans-serif;
        }

        .cardStyle {
            width: 500px;
            border-color: white;
            background: #fff;
            padding: 36px 0;
            border-radius: 4px;
            margin: 30px 0;
            box-shadow: 0px 0 2px 0 rgba(0,0,0,0.25);
        }

        #signupLogo {
            max-height: 100px;
            margin: auto;
            display: flex;
            flex-direction: column;
        }

        .formTitle {
            font-weight: 600;
            margin-top: 20px;
            color: #2F2D3B;
            text-align: center;
        }

        .inputLabel {
            font-size: 12px;
            color: #555;
            margin-bottom: 6px;
            margin-top: 24px;
        }

        .inputDiv {
            width: 70%;
            display: flex;
            flex-direction: column;
            margin: auto;
        }

        input {
            height: 40px;
            font-size: 16px;
            border-radius: 4px;
            border: none;
            border: solid 1px #ccc;
            padding: 0 11px;
        }

            input:disabled {
                cursor: not-allowed;
                border: solid 1px #eee;
            }

        .buttonWrapper {
            margin-top: 40px;
        }

        .submitButton {
            width: 70%;
            height: 40px;
            margin: auto;
            display: block;
            color: #fff;
            background-color: #065492;
            border-color: #065492;
            text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.12);
            box-shadow: 0 2px 0 rgba(0, 0, 0, 0.035);
            border-radius: 4px;
            font-size: 14px;
            cursor: pointer;
        }

            .submitButton:disabled,
            button[disabled] {
                border: 1px solid #cccccc;
                background-color: #cccccc;
                color: #666666;
            }

        #loader {
            position: absolute;
            z-index: 1;
            margin: -2px 0 0 10px;
            border: 4px solid #f3f3f3;
            border-radius: 50%;
            border-top: 4px solid #666666;
            width: 14px;
            height: 14px;
            -webkit-animation: spin 2s linear infinite;
            animation: spin 2s linear infinite;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
        /*end of css*/
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="mainDiv">
            <div class="cardStyle">

                <img src="" id="signupLogo" />

                <h2 class="formTitle">Login into SITCONNECT WEB APPLICATION
                </h2>
                <div class="inputDiv">
                    <label class="inputLabel" for="email">Email</label>
                    <asp:TextBox ID="tb_emaddr" runat="server" TextMode="Email"></asp:TextBox>
                    <asp:Label ID="lb_emaddr_errors" runat="server" ForeColor="Red"></asp:Label>

                </div>
                <div class="inputDiv">
                    <label class="inputLabel" for="password">Password</label>
                    <asp:TextBox ID="tb_pwd" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:Label ID="lbl_pwd_errors" runat="server" ForeColor="Red"></asp:Label>
                </div>

                <%--                <div class="inputDiv">
                    <label class="inputLabel" for="confirmPassword">Confirm Password</label>
                    <asp:TextBox ID="tb_confirmnewpassword" runat="server" TextMode="Password" onkeyup="javascript:validatepwd2()" maxlength="15"></asp:TextBox>
                      <asp:Label ID="lbl_cfmpassword_errors" runat="server" ForeColor="Red"></asp:Label>
                </div>--%>
                <asp:Label ID="lbl_error" runat="server" ForeColor="Red"></asp:Label>
                <div class="buttonWrapper">
                    <asp:Button ID="btn_Login" runat="server" Text="Login" OnClick="btn_Login_Submit" class="submitButton pure-button pure-button-primary" />
                </div>
                <br />

                <div class="buttonWrapper">
                    <h4 class="formTitle">Register if you do not have a Account.</h4>
                    <asp:Button ID="btn_Register" runat="server" Text="Register" OnClick="btn_Register_Submit" class="submitButton pure-button pure-button-primary" />
                </div>
            </div>
        </div>
        <div class="g-recaptcha" data-sitekey=" 6LeSlUgeAAAAAIluiMY7M_hJ-OmxxssZAR9oO4pu "></div>
        <%--        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/> --%>
        <asp:Label ID="Label1" runat="server" EnableViewState="False">Error Message Here (lblMessage)</asp:Label>
        &nbsp;&nbsp;&nbsp;
           <%-- called non breaking space --%>
        <br />
        <br />
        <asp:Label ID="lb_error" runat="server"></asp:Label>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LeSlUgeAAAAAIluiMY7M_hJ-OmxxssZAR9oO4pu', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
