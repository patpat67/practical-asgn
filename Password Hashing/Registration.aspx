<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Password_Hashing.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script>
        function setErrorFor(input) {
            const formControl = input.parentElement;
            const small = formControl.querySelector('small');
            formControl.className = 'form-control error';
        }

        function setSuccessFor(input) {
            const formControl = input.parentElement;
            formControl.className = 'form-control success';
        }
        function validatepassword() {
            var vpassword = document.getElementById("<%=tb_pwd.ClientID%>").value;
            if (vpassword.length < 8) {
                document.getElementById("<%=lbl_pwd_error.ClientID%>").innerHTML = "Password must be at least 8 characters in length.";
                // maybe style the tb?
                setErrorFor(vpassword);
            }
            else if (vpassword.length == 0) {
                document.getElementById("<%=lbl_pwd_error.ClientID%>").innerHTML = "Password must have upper and lowercase letters.";
            }
            else if (vpassword.search(/[a-z]/) == -1 || vpassword.search(/[A-Z]/) == -1) {
                document.getElementById("<%=lbl_pwd_error.ClientID%>").innerHTML = "Password must have upper and lowercase letters.";
            }
            else if (vpassword.search(/[0-9]/) == -1) {
                document.getElementById('<%=lbl_pwd_error.ClientID%>').innerHTML = "Password must contain 1 number!";
                document.getElementById('<%=tb_pwd.ClientID%>').style.background = "100px solid red";

            }
            else if (vpassword.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("<%=lbl_pwd_error.ClientID%>").innerHTML = "Password must have at least 1 special character.";
            }
            else {
                document.getElementById('<%=lbl_pwd_error.ClientID%>').innerHTML = "";
                document.getElementById('<%=tb_pwd.ClientID%>').style.border = "";
                return true
            }
        }
        function validatefirstname() {
            var vfname = document.getElementById("<%=tb_fname%>").value;
            if (vfname.length == 0) {
                document.getElementById('<%=lbl_fname_errors.ClientID%>').innerHTML = "First name is required!";
            }
            else if (vfname.search(/^[A-Za-z]+$/) == -1) {
                document.getElementById('<%=lbl_fname_errors.ClientID%>').innerHTML = "Only Alphabets (A-Z) allowed!";
            } else {
                document.getElementById('<%=lbl_fname_errors.ClientID%>').innerHTML = "";
                return true
            }
        }
        function validateLastName() {
            var vlname = document.getElementById("<%=tb_lname%>").value;
            if (vlname.length == 0) {
                document.getElementById('<%=lbl_lname_errors.ClientID%>').innerHTML = "First name is required!";
            }
            else if (vlname.search(/^[A-Za-z]+$/) == -1) {
                document.getElementById('<%=lbl_lname_errors.ClientID%>').innerHTML = "Only Alphabets (A-Z) allowed!";
            } else {
                document.getElementById('<%=lbl_lname_errors.ClientID%>').innerHTML = "";
                return true
            }
        }
        function validateCreditCard() {
            var cci = document.getElementById("<%=tb_cci%>").value;
            if (cci.length == 0) {
                document.getElementById('<%=lbl_cc_errors.ClientID%>').innerHTML = "Credit Card Information is required!";
            }
            else if (cci.search(/^[0-9]*$/) == -1) {
                document.getElementById('<%=lbl_cc_errors.ClientID%>').innerHTML = "Only numbers(0-9) are allowed!";
            } else {
                document.getElementById('<%=lbl_cc_errors.ClientID%>').innerHTML = "";
                return true
            }
        }
        function validateEmail() {
            var vemail = document.getElementById("<%=tb_email%>").value;

            if (vemail.length == 0) {
                document.getElementById('<%=lb_emaddr_errors.ClientID%>').innerHTML = "Email is required!";
            }
            else if (vemail.search(/^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/) == -1) {
                document.getElementById('<%=lb_emaddr_errors.ClientID%>').innerHTML = "Please use a valid email!";
            } else {
                document.getElementById('<%=lb_emaddr_errors.ClientID%>').innerHTML = "";
                return true
            }
        }
        function validateconfirmpass() {
            var vcpassword = document.getElementById("<%=tb_cpassword%>").value;
            var vpassword = document.getElementById("<%=tb_pwd%>").value;
            if (vcpassword.length == 0) {
                document.getElementById('<%=lbl_cpassword_errors.ClientID%>').innerHTML = "You need to confirm your password before registering!";

            }

            else if (vpassword != vcpassword) {
                document.getElementById('<%=lbl_cpassword_errors.ClientID%>').innerHTML = "Password does not match with the one u keyed in earlier!";

            } else {
                document.getElementById('<%=lbl_cpassword_errors.ClientID%>').innerHTML = "";
                return true
            }
        }
        function validatedob() {
            var vdob = document.getElementById("<%=tb_dob%>").value;
            if (vdob.length == 0) {
                document.getElementById('<%=lbl_dob_errors.ClientID%>').innerHTML = "Date Of Birth is required!";
                document.getElementById('<%=tb_dob.ClientID%>').style.border = "1px solid red";

            } else {
                document.getElementById('<%=lbl_dob_errors.ClientID%>').innerHTML = "";
                document.getElementById('<%=tb_dob.ClientID%>').style.border = "";
                return true
            }
        }

    </script>
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

                <h2 class="formTitle">Account Registration
                </h2>
                <div class="inputDiv">
                    <label class="inputLabel">First Name</label>
                    <asp:TextBox ID="tb_fname" runat="server" onkeyup="javascript:validatefirstname()" MaxLength="30"></asp:TextBox>
                    <asp:Label ID="lbl_fname_errors" runat="server" ForeColor="Red"></asp:Label>

                </div>
                <div class="inputDiv">
                    <label class="inputLabel">Last Name</label>
                    <asp:TextBox ID="tb_lname" runat="server"  onkeyup="javascript:validateLastName()"></asp:TextBox>
                    <asp:Label ID="lbl_lname_errors" runat="server" ForeColor="Red"></asp:Label>
                </div>

                <div class="inputDiv">
                    <label class="inputLabel">Credit Card Information</label>
                    <asp:TextBox ID="tb_cci" runat="server"  onkeyup="javascript:validateCreditCard()" ></asp:TextBox>
                    <asp:Label ID="lbl_cc_errors" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <div class="inputDiv">
                    <label class="inputLabel">Email Address</label>
                    <asp:TextBox ID="tb_email" runat="server"  onkeyup="javascript:validateEmail()" ></asp:TextBox>
                    <asp:Label ID="lb_emaddr_errors" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <div class="inputDiv">
                    <label class="inputLabel">Password</label>
                    <asp:TextBox ID="tb_pwd" runat="server" TextMode="Password" onkeyup="javascript:validatepassword()" ></asp:TextBox>
                    <asp:Label ID="lbl_pwd_error" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <div class="inputDiv">
                    <label class="inputLabel">Confirm Password</label>
                    <asp:TextBox ID="tb_cpassword" runat="server" TextMode="Password" onkeyup="javascript:validateconfirmpass()" ></asp:TextBox>
                    <asp:Label ID="lbl_cpassword_errors" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <div class="inputDiv">
                    <label class="inputLabel">Date of Birth</label>
                    <asp:TextBox ID="tb_dob" runat="server"  onkeyup="javascript:validatedob()"></asp:TextBox>
                    <asp:Label ID="lbl_dob_errors" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <br />
                <br />
                <div class="inputDiv">
                    Image file to upload to the server:
                        <input id="oFile" type="file" runat="server" name="oFile" />
                    <asp:Button ID="btnUpload" type="submit" Text="Upload" runat="server"></asp:Button>
                    <br />
                    <asp:Panel ID="frmConfirmation" Visible="False" runat="server">
                        <asp:Label ID="lblUploadResult" runat="server"></asp:Label>
                    </asp:Panel>
                </div>

                <asp:Label ID="lbl_error" runat="server" ForeColor="Red"></asp:Label>
                <div class="buttonWrapper">
                    <asp:Button ID="btn_Register" runat="server" Text="Register" OnClick="btn_Register_Click" class="submitButton pure-button pure-button-primary" />
                </div>

            </div>
        </div>
    </form>
</body>
</html>


