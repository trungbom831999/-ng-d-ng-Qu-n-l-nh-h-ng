Create database QLNhaHang;

Use QLNhaHang;

--TableInfo
--Account
--Food
--FoodCategory
--Bill
--BillInfo

Create Table TableInfo
(
	id Int Identity Primary key,
	name Nvarchar(100) not NULL default N'Bàn chưa đặt tên',
	statusTable Nvarchar(100) not NULL default N'Trống', --trống || có khách
	type int not NULL default 1
);

Create Table Account
(
	userName Nvarchar(100) Primary key,
	displayName Nvarchar(100) not NULL default N'Member',
	password Nvarchar(1000) not NULL default N'1',
	type int not NULL default 0 -- 1 là admin; 0 là staff
);

Create Table FoodCategory
(
	id Int Identity Primary key,
	name Nvarchar(100) not NULL default N'Chưa đặt tên thể loại',
	type int not NULL default 1
);

Create Table Food
(
	id Int Identity Primary key,
	name Nvarchar(100) not NULL default N'Chưa đặt tên món',
	idCategory Int not NULL,
	price float not NULL

	Foreign key (idCategory) references dbo.FoodCategory(id)
);

Create Table Bill
(
	id Int Identity Primary key,
	dateCheckIn Date not NULL default Getdate(),
	dateCheckOut Date,
	idTable Int not NULL,
	statusBill Int not NULL default 0 -- 1 là đã thanh toán; 0 là chưa thanh toán
	
	Foreign key (idTable) references dbo.TableInfo(id)
);

Create Table BillInfo
(
	id Int Identity Primary key,
	idBill Int not NULL,
	idFood Int not NULL,
	count Int not NULL default 0

	Foreign key (idBill) references dbo.Bill(id),
	Foreign key (idFood) references dbo.Food(id)
);

Create Proc USP_GetAccountByUserName
@userName Nvarchar(100)
As
Begin
	Select * From Account
	Where userName = @userName
End;

Exec USP_GetAccountByUserName @userName = N'n';

Create Proc USP_Login
@userName Nvarchar(100) , @password Nvarchar(100)
As
Begin
	Select * From Account
	Where userName = @userName
	And password = @password
End;

Alter Proc USP_TableList
As Select * From TableInfo Where type = 1;

Exec USP_TableList;

Create Proc USP_InsertBill
@idTable int
As
Begin
	Insert Bill
		(dateCheckIn,
		 dateCheckOut,
		 idTable,
		 statusBill
		)
	Values 
		(Getdate(), --dateCheckIn = Date in Computer
		 Null,
		 @idTable,
		 0
		 )
End;

Create Proc USP_InsertBillInfo
@idBill int, @idFood int, @count int
As
Begin
	Declare @isExistBillInfo int
	Declare @foodCount int = 1

	Select @isExistBillInfo = id, @foodCount = count
	From BillInfo
	Where idBill = @idBill 
	  and idFood = @idFood

	if(@isExistBillInfo > 0)
	Begin
		Declare @newCount int = @foodCount + @count
		if(@newCount > 0)
			Update BillInfo 
			Set count = @foodCount + @count
			Where idFood = @idFood
		else
			Delete BillInfo Where idBill = @idBill 
							  and idFood = @idFood
	End
	else
	Begin
		Insert BillInfo
			(idBill,
			 idFood,
			 count
			)
		Values 
			(@idBill,
			 @idFood,
			 @count
			 )
	End
End;

Create Trigger UTG_UpdateBillInfo
On BillInfo For Insert, Update
As
Begin
	Declare @idBill int
	Select @idBill = idBill 
	From Inserted

	Declare @idTable int
	Select @idTable = idTable 
	From Bill 
	Where id = @idBill
	  and statusBill = 0

	Declare @count int
	Select @count = Count(*) From BillInfo
	Where idBill = @idBill

	If(@count > 0)
	Begin
		Update TableInfo 
		Set statusTable = N'Có Khách'
		Where id = @idTable
	End
	Else
	Begin
		Update TableInfo 
		Set statusTable = N'Trống'
		Where id = @idTable
	End
End;
					
Create Trigger UTG_UpdateBill
On Bill For Update
As
Begin
	Declare @idBill int
	Select @idBill = id
	From Inserted

	Declare @idTable int
	Select @idTable = idTable 
	From Bill
	Where id = @idBill

	Declare @count int = 0
	Select @count = Count(*)
	From Bill
	Where idTable = @idTable
	  and statusBill = 0

	If(@count = 0) Update TableInfo Set statusTable = N'Trống'
	Where id = @idTable
End;

Create Proc USP_SwitchTable
@idTable1 int, @idTable2 int
As
Begin
	Declare @idFirstBill int
	Declare @idSecondBill int

	Declare @idFirstTableEmty int = 1
	Declare @idSecondTableEmty int = 1

	Select @idFirstBill = id From Bill 
	Where idTable = @idTable1 and statusBill = 0
	Select @idSecondBill = id From Bill 
	Where idTable = @idTable2 and statusBill = 0

	If(@idFirstBill is null)
	Begin
		Insert Bill
		(dateCheckIn,
		 dateCheckOut,
		 idTable,
		 statusBill
		)
	Values 
		(Getdate(), --dateCheckIn = Date in Computer
		 Null,
		 @idTable1,
		 0 
		 )
		 Select @idFirstBill = MAX(id) From Bill Where idTable = @idTable1 and statusBill = 0
	End

	Select @idFirstTableEmty = Count(*) From BillInfo
										Where idBill = @idFirstBill

	If(@idSecondBill is null)
	Begin
		Insert Bill
		(dateCheckIn,
		 dateCheckOut,
		 idTable,
		 statusBill
		)
	Values 
		(Getdate(), --dateCheckIn = Date in Computer
		 Null,
		 @idTable2,
		 0 
		 )
		 Select @idSecondBill = MAX(id) From Bill Where idTable = @idTable2 and statusBill = 0
	End

	Select @idSecondTableEmty = Count(*) From BillInfo
										Where idBill = @idSecondBill

	Select id Into IdBillInfoTable --cop sang trung gian
	From BillInfo
	Where idBill = @idSecondBill
	Update BillInfo Set idBill = @idSecondBill 
	Where idBill = @idFirstBill
	Update BillInfo Set idBill = @idFirstBill 
	Where id in (Select * From IdBillInfoTable)
	Drop Table IdBillInfoTable

	If(@idFirstTableEmty = 0)
		Update TableInfo Set statusTable = N'Trống' Where id = @idTable2
	If(@idSecondTableEmty = 0)
		Update TableInfo Set statusTable = N'Trống' Where id = @idTable1
End;

Alter Table Bill Add totalPrice nvarchar(100);

Create Proc USP_GetListBillByDate
@checkIn date, @checkOut date
As
Begin
	Select Distinct Bill.id, dateCheckIn as [Ngày vào], dateCheckOut as [Ngày ra], totalPrice as [Tổng hóa đơn]
	From Bill
	Where dateCheckIn >= @checkIn and dateCheckOut <= @checkOut
	and statusBill = 1;
End;

Create Proc USP_GetListBillByDateAndPage
@checkIn date, @checkOut date, @page int
As
Begin
	Declare @pageRows int = 10
	Declare @selectRows int = @pageRows * @page
	Declare @exceptRows int = (@page - 1) * @pageRows
	
	;With BillShow as (Select Distinct Bill.id, dateCheckIn as [Ngày vào], dateCheckOut as [Ngày ra], Format(totalPrice, '#,000') as [Tổng hóa đơn]
	From Bill
	Where dateCheckIn >= @checkIn and dateCheckOut <= @checkOut
	and statusBill = 1)
	Select Top (@selectRows) * From BillShow
	Except
	Select Top (@exceptRows) * From BillShow
End;

Create Proc USP_GetCountBillByDate
@checkIn date, @checkOut date
As
Begin
	Select Distinct Count (*) 
	From Bill
	Where dateCheckIn >= @checkIn and dateCheckOut <= @checkOut
	and statusBill = 1;
End;

Create Proc USP_UpdateAccount
@userName nvarchar(100), @displayName nvarchar(100), @password nvarchar(100), @newPassword nvarchar(100)
As
Begin
	Declare @isRightPass int
	Select @isRightPass = Count (*) From Account
	Where userName = @userName and password = @password
	If(@isRightPass = 1)
	Begin
		If(@newPassword = null or @newPassword = ' ' )
			Update Account Set displayName = @displayName Where userName = @userName
		Else
			Update Account Set displayName = @displayName, password = @newPassword Where userName = @userName

	End
End;

Create Proc USP_GetListFood
As 
Begin
	Select Food.id as [ID], Food.name as [Tên món], FoodCategory.name as [Thể loại], price as [Đơn giá] 
	From Food, FoodCategory
	Where Food.idCategory = FoodCategory.id;
End;

Exec USP_GetListFood;

Create Trigger UTG_DeletebillInfo
On BillInfo For Delete
As
Begin
	Declare @idBillInfo int
	Declare @idBill int
	Select @idBillInfo = id, @idBill =  deleted.idBill From deleted

	Declare @idTable int
	Select @idTable = idTable From Bill Where id = @idBill

	Declare @count int = 0
	Select @count = Count (*) From BillInfo, Bill
	Where Bill.id = BillInfo.idBill and Bill.id = @idBill and statusBill = 0

	If(@count = 0)
		Update TableInfo Set statusTable = N'Trống' Where id = @idTable
End

Create FUNCTION [dbo].[fuConvertToUnsign] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS BEGIN IF @strInput IS NULL RETURN @strInput IF @strInput = '' RETURN @strInput DECLARE @RT NVARCHAR(4000) DECLARE @SIGN_CHARS NCHAR(136) DECLARE @UNSIGN_CHARS NCHAR (136) SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END;

--Select * From Bill Where statusBill = 0;

/*Select Food.name, count, price, price*count as totalPrice
From Food, Bill, BillInfo, TableInfo
Where Food.id = BillInfo.idFood
And Bill.id = BillInfo.idBill
And Bill.idTable = TableInfo.id
And Bill.idTable = 5;*/

--Update Bill Set statusBill = 1 Where id = 5;
/*Delete BillInfo
Delete Bill*/

/*Select * From Bill;
Select * From TableInfo;
Delete BillInfo;
Delete Bill;
Update TableInfo Set statusTable = N'Trống';*/

/*Select Food.name as [Tên món], FoodCategory.name as [Thể loại], Format(price, '#,000 VND') as [Đơn giá] 
From Food, FoodCategory
Where Food.idCategory = FoodCategory.id;*/

/*Select Food.name, count, price  From Food, Bill, BillInfo Where Food.id = BillInfo.idFood And Bill.id = BillInfo.idBill And Bill.id = 2;
Select Food.name, count, price, price*count as totalPrice From Food, Bill, BillInfo, TableInfo Where Food.id = BillInfo.idFood And Bill.id = BillInfo.idBill And Bill.idTable = TableInfo.id And Bill.id = 1;
Select * From Food Where name = N'Salad';*/

/*Tìm kiếm gần đúng
Select Food.id as [ID], Food.name as [Tên món], FoodCategory.name as [Thể loại], price as [Đơn giá] 
From Food, FoodCategory
Where Food.idCategory = FoodCategory.id
and dbo.fuConvertToUnsign(Food.name) like N'%' + dbo.fuConvertToUnsign(N'Sả') + '%';*/

/*Select * From FoodCategory
Insert FoodCategory(name, type) Values (N'abc', 1)
Select * From TableInfo
Select * From FoodCategory Where type = 1
Delete FoodCategory Where id = 11;*/






