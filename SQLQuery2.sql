create table BSIT_StudentInformationSheetCompilation(
Id int primary key identity,
bsit_course_id int foreign key references tbl_BSITSTud_CourseRemarks(bsit_course_id),
bsit_studaddress_id int foreign key references tbl_BSITSTud_Address(bsit_studaddress_id),
bsit_studcontact_id int foreign key references tbl_BSITSTud_ContactDetail(bsit_studcontact_id),
bsit_studcred_id int foreign key references tbl_BSITSTud_Credentials(bsit_studcred_id),
bsit_studprev_id int foreign key references tbl_BSITSTud_SchoolBackground(bsit_studprev_id),
bsit_studid varchar,
bsit_studfname varchar,
bsit_studmname varchar,
bsit_studsuffix varchar,
)
