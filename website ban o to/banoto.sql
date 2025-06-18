-- ===================================================
-- DATABASE WEBSITE BÁN Ô TÔ - SQL SERVER
-- Car Dealership Management System
-- ===================================================

-- Create database
CREATE DATABASE BanOto;
GO

USE BanOto;
GO

-- ===================================================
-- 1. BẢNG NGƯỜI DÙNG (Users)
-- ===================================================
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20),
    Role NVARCHAR(20) NOT NULL DEFAULT 'User', -- Admin, Staff, User
    IsAdmin BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME NULL
);

-- ===================================================
-- 2. BẢNG KHU VỰC (Locations)
-- ===================================================
CREATE TABLE Locations (
    LocationID INT IDENTITY(1,1) PRIMARY KEY,
    LocationName NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- ===================================================
-- 3. BẢNG HÃNG XE (Brands)
-- ===================================================
CREATE TABLE Brands (
    BrandID INT IDENTITY(1,1) PRIMARY KEY,
    BrandName NVARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- ===================================================
-- 4. BẢNG XE (Cars)
-- ===================================================
CREATE TABLE Cars (
    CarID INT IDENTITY(1,1) PRIMARY KEY,
    BrandID INT NOT NULL,
    LocationID INT NULL,
    CarName NVARCHAR(100) NOT NULL,
    Price DECIMAL(15,0) NOT NULL, -- Giá tính bằng triệu VND
    Description NTEXT NULL,
    Year INT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    IsDisplay BIT NOT NULL DEFAULT 1, -- Hiển thị trên website
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME NULL,
    FOREIGN KEY (BrandID) REFERENCES Brands(BrandID),
    FOREIGN KEY (LocationID) REFERENCES Locations(LocationID)
);

-- ===================================================
-- 5. BẢNG HÌNH ẢNH XE (CarImages)
-- ===================================================
CREATE TABLE CarImages (
    ImageID INT IDENTITY(1,1) PRIMARY KEY,
    CarID INT NOT NULL,
    ImagePath NVARCHAR(255) NOT NULL,
    ImageName NVARCHAR(100) NULL,
    IsMain BIT NOT NULL DEFAULT 0, -- Ảnh chính
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CarID) REFERENCES Cars(CarID) ON DELETE CASCADE
);

-- ===================================================
-- 6. BẢNG ĐƠN HÀNG (Orders)
-- ===================================================
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderCode NVARCHAR(20) NOT NULL UNIQUE, -- DH01, DH02, DH03...
    UserID INT NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    CustomerEmail NVARCHAR(100) NULL,
    CustomerPhone NVARCHAR(20) NULL,
    TotalAmount DECIMAL(15,0) NOT NULL,
    OrderStatus NVARCHAR(20) NOT NULL DEFAULT N'Đang xử lý', -- Đang xử lý, Đã giao, Đã hủy
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    DeliveryDate DATETIME NULL,
    Notes NTEXT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- ===================================================
-- 7. BẢNG CHI TIẾT ĐƠN HÀNG (OrderDetails)
-- ===================================================
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    CarID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    UnitPrice DECIMAL(15,0) NOT NULL,
    TotalPrice DECIMAL(15,0) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (CarID) REFERENCES Cars(CarID)
);

-- ===================================================
-- 8. BẢNG GIỎ HÀNG (Cart)
-- ===================================================
CREATE TABLE Cart (
    CartID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    CarID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    AddedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (CarID) REFERENCES Cars(CarID) ON DELETE CASCADE
);

-- ===================================================
-- 9. BẢNG TIN TỨC (News)
-- ===================================================
CREATE TABLE News (
    NewsID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Slug NVARCHAR(200) NOT NULL UNIQUE,
    Summary NVARCHAR(500) NULL,
    Content NTEXT NULL,
    ImagePath NVARCHAR(255) NULL,
    IsPublished BIT NOT NULL DEFAULT 1,
    PublishedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME NULL,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserID)
);

-- ===================================================
-- 10. BẢNG LIÊN HỆ (Contacts)
-- ===================================================
CREATE TABLE Contacts (
    ContactID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Subject NVARCHAR(200) NULL,
    Message NTEXT NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ReplyDate DATETIME NULL,
    ReplyMessage NTEXT NULL
);

-- ===================================================
-- 11. BẢNG TIN BÁN XE CŨ (UsedCarPosts)
-- ===================================================
CREATE TABLE UsedCarPosts (
    PostID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NULL,
    CarName NVARCHAR(100) NOT NULL,
    ExpectedPrice DECIMAL(15,0) NULL,
    Description NTEXT NULL,
    ContactName NVARCHAR(100) NOT NULL,
    ContactEmail NVARCHAR(100) NULL,
    ContactPhone NVARCHAR(20) NULL,
    ImagePath NVARCHAR(255) NULL,
    IsApproved BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ApprovedDate DATETIME NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- ===================================================
-- CHÈN DỮ LIỆU MẪU (Sample Data)
-- ===================================================

-- Insert Users
INSERT INTO Users (Username, Password, FullName, Email, Phone, Role, IsAdmin, IsActive) VALUES 
('admin', 'admin123', N'Quản trị viên', 'admin@oto.vn', '0987654321', 'Admin', 1, 1),
('user1', 'user123', N'Nguyễn Văn A', 'user1@example.com', '0912345678', 'User', 0, 1),
('staff1', 'staff123', N'Nhân viên B', 'staff@oto.vn', '0923456789', 'Staff', 0, 1);

-- Insert Locations
INSERT INTO Locations (LocationName, IsActive) VALUES 
(N'Toàn quốc', 1),
(N'Hà Nội', 1),
(N'TP. Hồ Chí Minh', 1),
(N'Đà Nẵng', 1),
(N'Hải Phòng', 1),
(N'Cần Thơ', 1);

-- Insert Brands
INSERT INTO Brands (BrandName, IsActive) VALUES 
('Audi', 1),
('BMW', 1),
('Chevrolet', 1),
('Ford', 1),
('Honda', 1),
('Hyundai', 1),
('Kia', 1),
('Toyota', 1),
('VinFast', 1),
('Mazda', 1);

-- Insert Cars
INSERT INTO Cars (BrandID, LocationID, CarName, Price, Description, Year, IsAvailable, IsDisplay) VALUES 
(8, 2, 'Toyota Camry 2023', 1200, N'Sedan hạng sang với động cơ 2.5L', 2023, 1, 1),
(5, 1, 'Honda CR-V 2022', 950, N'SUV 5 chỗ tiết kiệm nhiên liệu', 2022, 1, 1),
(9, 3, 'VinFast VF8', 1200, N'SUV điện thông minh', 2023, 1, 1),
(8, 2, 'Toyota Vios', 550, N'Sedan phổ thông 4 chỗ', 2023, 1, 1),
(10, 1, 'Mazda CX5', 850, N'SUV 5 chỗ thiết kế thể thao', 2023, 1, 1),
(9, 3, 'VinFast Lux A2.0', 880, N'Sedan sang trọng', 2022, 1, 1);

-- Insert CarImages
INSERT INTO CarImages (CarID, ImagePath, ImageName, IsMain) VALUES 
(1, '/images/cars/camry-2023-1.jpg', 'Toyota Camry 2023 - Main', 1),
(1, '/images/cars/camry-2023-2.jpg', 'Toyota Camry 2023 - Interior', 0),
(2, '/images/cars/crv-2022-1.jpg', 'Honda CR-V 2022 - Main', 1),
(3, '/images/cars/vf8-2023-1.jpg', 'VinFast VF8 - Main', 1),
(4, '/images/cars/vios-2023-1.jpg', 'Toyota Vios - Main', 1),
(5, '/images/cars/cx5-2023-1.jpg', 'Mazda CX5 - Main', 1),
(6, '/images/cars/lux-a20-1.jpg', 'VinFast Lux A2.0 - Main', 1);

-- Insert Orders
INSERT INTO Orders (OrderCode, UserID, CustomerName, CustomerEmail, CustomerPhone, TotalAmount, OrderStatus, OrderDate) VALUES 
('DH001', 2, N'Nguyễn Văn A', 'nguyenvana@email.com', '0912345678', 550, N'Đã giao', '2025-01-02'),
('DH002', 2, N'Trần Thị B', 'tranthib@email.com', '0923456789', 850, N'Đang xử lý', '2025-01-05'),
('DH003', 3, N'Lê Văn C', 'levanc@email.com', '0934567890', 880, N'Đã giao', '2025-01-12');

-- Insert OrderDetails
INSERT INTO OrderDetails (OrderID, CarID, Quantity, UnitPrice, TotalPrice) VALUES 
(1, 4, 1, 550, 550), -- DH001: Toyota Vios
(2, 5, 1, 850, 850), -- DH002: Mazda CX5  
(3, 6, 1, 880, 880); -- DH003: VinFast Lux A2.0

-- Insert News
INSERT INTO News (Title, Slug, Summary, Content, ImagePath, CreatedBy) VALUES 
(N'VinFast ra mắt mẫu xe điện mới', 'vinfast-ra-mat-mau-xe-dien-moi', 
 N'Mẫu xe điện mới nhất của VinFast có thiết kế hiện đại...', 
 N'VinFast vừa chính thức giới thiệu mẫu xe điện mới với nhiều cải tiến vượt trội...', 
 '/images/news/vinfast-new-1.jpg', 1),
(N'Honda Civic 2025 trình làng', 'honda-civic-2025-trinh-lang',
 N'Honda chính thức giới thiệu Civic 2025 với nhiều nâng cấp...',
 N'Mẫu sedan Honda Civic thế hệ mới 2025 đã được Honda chính thức ra mắt...',
 '/images/news/civic-2025-1.jpg', 1);

-- Insert Contacts
INSERT INTO Contacts (FullName, Email, Phone, Subject, Message, IsRead) VALUES 
(N'Nguyễn Văn Đức', 'duc@email.com', '0945123456', N'Hỏi về xe Toyota Camry', N'Tôi muốn biết thêm về xe Toyota Camry 2023', 0),
(N'Trần Thị Mai', 'mai@email.com', '0956234567', N'Lịch hẹn xem xe', N'Tôi muốn đặt lịch xem xe Honda CR-V', 1);

-- ===================================================
-- TẠO CÁC INDEX ĐỂ TỐI ƯU HIỆU SUẤT
-- ===================================================

-- Index cho tìm kiếm xe theo hãng
CREATE INDEX IX_Cars_BrandID ON Cars(BrandID);

-- Index cho tìm kiếm xe theo khu vực
CREATE INDEX IX_Cars_LocationID ON Cars(LocationID);

-- Index cho tìm kiếm xe theo giá
CREATE INDEX IX_Cars_Price ON Cars(Price);

-- Index cho đơn hàng theo người dùng
CREATE INDEX IX_Orders_UserID ON Orders(UserID);

-- Index cho đơn hàng theo ngày
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);

-- Index cho chi tiết đơn hàng
CREATE INDEX IX_OrderDetails_OrderID ON OrderDetails(OrderID);
CREATE INDEX IX_OrderDetails_CarID ON OrderDetails(CarID);

-- Index cho giỏ hàng
CREATE INDEX IX_Cart_UserID ON Cart(UserID);

-- Index cho tin tức
CREATE INDEX IX_News_PublishedDate ON News(PublishedDate);
CREATE INDEX IX_News_IsPublished ON News(IsPublished);

-- ===================================================
-- TẠO CÁC VIEW ĐỂ BÁO CÁO VÀ THỐNG KÊ
-- ===================================================
go
-- View: Thống kê doanh thu
CREATE VIEW vw_RevenueStatistics AS
SELECT 
    COUNT(*) as TotalOrders,
    SUM(TotalAmount) as TotalRevenue,
    MAX(TotalAmount) as HighestOrder,
    MIN(TotalAmount) as LowestOrder,
    AVG(TotalAmount) as AverageOrder
FROM Orders 
WHERE OrderStatus = N'Đã giao';
GO

-- View: Top sản phẩm bán chạy
CREATE VIEW vw_TopSellingCars AS
SELECT 
    c.CarID,
    c.CarName,
    b.BrandName,
    SUM(od.Quantity) as TotalSold,
    SUM(od.TotalPrice) as TotalRevenue
FROM Cars c
INNER JOIN OrderDetails od ON c.CarID = od.CarID
INNER JOIN Orders o ON od.OrderID = o.OrderID
INNER JOIN Brands b ON c.BrandID = b.BrandID
WHERE o.OrderStatus = N'Đã giao'
GROUP BY c.CarID, c.CarName, b.BrandName;
GO

-- View: Danh sách xe với thông tin đầy đủ
CREATE VIEW vw_CarsWithDetails AS
SELECT 
    c.CarID,
    c.CarName,
    b.BrandName,
    l.LocationName,
    c.Price,
    c.Year,
    c.IsAvailable,
    c.IsDisplay,
    ci.ImagePath as MainImage
FROM Cars c
INNER JOIN Brands b ON c.BrandID = b.BrandID
LEFT JOIN Locations l ON c.LocationID = l.LocationID
LEFT JOIN CarImages ci ON c.CarID = ci.CarID AND ci.IsMain = 1;
GO

-- View: Chi tiết đơn hàng
CREATE VIEW vw_OrderDetails AS
SELECT 
    o.OrderID,
    o.OrderCode,
    o.CustomerName,
    o.OrderDate,
    o.TotalAmount,
    o.OrderStatus,
    c.CarName,
    b.BrandName,
    od.Quantity,
    od.UnitPrice
FROM Orders o
INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
INNER JOIN Cars c ON od.CarID = c.CarID
INNER JOIN Brands b ON c.BrandID = b.BrandID;
GO

-- ===================================================
-- TẠO CÁC STORED PROCEDURE
-- ===================================================

-- Stored Procedure: Tìm kiếm xe
CREATE PROCEDURE sp_SearchCars
    @BrandID INT = NULL,
    @LocationID INT = NULL,
    @MinPrice DECIMAL(15,0) = NULL,
    @MaxPrice DECIMAL(15,0) = NULL,
    @Keyword NVARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        c.CarID,
        c.CarName,
        b.BrandName,
        l.LocationName,
        c.Price,
        c.Year,
        c.Description,
        ci.ImagePath as MainImage
    FROM Cars c
    INNER JOIN Brands b ON c.BrandID = b.BrandID
    LEFT JOIN Locations l ON c.LocationID = l.LocationID
    LEFT JOIN CarImages ci ON c.CarID = ci.CarID AND ci.IsMain = 1
    WHERE c.IsAvailable = 1 AND c.IsDisplay = 1
        AND (@BrandID IS NULL OR c.BrandID = @BrandID)
        AND (@LocationID IS NULL OR c.LocationID = @LocationID)
        AND (@MinPrice IS NULL OR c.Price >= @MinPrice)
        AND (@MaxPrice IS NULL OR c.Price <= @MaxPrice)
        AND (@Keyword IS NULL OR c.CarName LIKE N'%' + @Keyword + '%')
    ORDER BY c.CreatedDate DESC;
END;
GO

-- Stored Procedure: Thêm xe vào giỏ hàng
CREATE PROCEDURE sp_AddToCart
    @UserID INT,
    @CarID INT,
    @Quantity INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Kiểm tra xe có tồn tại và còn hàng không
    IF NOT EXISTS (SELECT 1 FROM Cars WHERE CarID = @CarID AND IsAvailable = 1)
    BEGIN
        SELECT 'Error' as Result, N'Xe không tồn tại hoặc đã hết hàng' as Message;
        RETURN;
    END
    
    -- Kiểm tra xe đã có trong giỏ hàng chưa
    IF EXISTS (SELECT 1 FROM Cart WHERE UserID = @UserID AND CarID = @CarID)
    BEGIN
        -- Cập nhật số lượng
        UPDATE Cart 
        SET Quantity = Quantity + @Quantity,
            AddedDate = GETDATE()
        WHERE UserID = @UserID AND CarID = @CarID;
        
        SELECT 'Updated' as Result, N'Đã cập nhật số lượng trong giỏ hàng' as Message;
    END
    ELSE
    BEGIN
        -- Thêm mới vào giỏ hàng
        INSERT INTO Cart (UserID, CarID, Quantity)
        VALUES (@UserID, @CarID, @Quantity);
        
        SELECT 'Added' as Result, N'Đã thêm vào giỏ hàng' as Message;
    END
END;
GO

-- Stored Procedure: Tạo đơn hàng từ giỏ hàng
CREATE PROCEDURE sp_CreateOrderFromCart
    @UserID INT,
    @CustomerName NVARCHAR(100),
    @CustomerEmail NVARCHAR(100) = NULL,
    @CustomerPhone NVARCHAR(20) = NULL,
    @Notes NTEXT = NULL
AS
BEGIN
    DECLARE @OrderID INT;
    DECLARE @OrderCode NVARCHAR(20);
    DECLARE @TotalAmount DECIMAL(15,0);
    
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Tính tổng tiền
        SELECT @TotalAmount = SUM(c.Price * cart.Quantity)
        FROM Cart cart
        INNER JOIN Cars c ON cart.CarID = c.CarID
        WHERE cart.UserID = @UserID AND c.IsAvailable = 1;
        
        -- Kiểm tra giỏ hàng có sản phẩm không
        IF @TotalAmount IS NULL OR @TotalAmount = 0
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 'Error' as Result, N'Giỏ hàng trống hoặc không có sản phẩm khả dụng!' as Message;
            RETURN;
        END
        
        -- Tạo mã đơn hàng
        DECLARE @NextOrderNum INT;
        SELECT @NextOrderNum = ISNULL(MAX(CAST(SUBSTRING(OrderCode, 3, 10) AS INT)), 0) + 1 
        FROM Orders 
        WHERE OrderCode LIKE 'DH%';
        
        SET @OrderCode = 'DH' + RIGHT('000' + CAST(@NextOrderNum AS VARCHAR(3)), 3);
        
        -- Tạo đơn hàng
        INSERT INTO Orders (OrderCode, UserID, CustomerName, CustomerEmail, CustomerPhone, TotalAmount, Notes)
        VALUES (@OrderCode, @UserID, @CustomerName, @CustomerEmail, @CustomerPhone, @TotalAmount, @Notes);
        
        SET @OrderID = SCOPE_IDENTITY();
        
        -- Tạo chi tiết đơn hàng từ giỏ hàng
        INSERT INTO OrderDetails (OrderID, CarID, Quantity, UnitPrice, TotalPrice)
        SELECT 
            @OrderID,
            cart.CarID,
            cart.Quantity,
            c.Price,
            c.Price * cart.Quantity
        FROM Cart cart
        INNER JOIN Cars c ON cart.CarID = c.CarID
        WHERE cart.UserID = @UserID AND c.IsAvailable = 1;
        
        -- Xóa giỏ hàng
        DELETE FROM Cart WHERE UserID = @UserID;
        
        COMMIT TRANSACTION;
        
        SELECT 'Success' as Result, @OrderID as OrderID, @OrderCode as OrderCode, @TotalAmount as TotalAmount;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        SELECT 'Error' as Result, @ErrorMessage as Message;
    END CATCH
END;
GO

-- Stored Procedure: Cập nhật trạng thái đơn hàng
CREATE PROCEDURE sp_UpdateOrderStatus
    @OrderID INT,
    @OrderStatus NVARCHAR(20),
    @DeliveryDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Orders 
    SET OrderStatus = @OrderStatus,
        DeliveryDate = @DeliveryDate,
        OrderDate = GETDATE()
    WHERE OrderID = @OrderID;
    
    IF @@ROWCOUNT > 0
        SELECT 'Success' as Result, N'Cập nhật trạng thái đơn hàng thành công' as Message;
    ELSE
        SELECT 'Error' as Result, N'Không tìm thấy đơn hàng' as Message;
END;
GO

-- ===================================================
-- FUNCTION TẠO MÃ ĐƠN HÀNG TỰ ĐỘNG
-- ===================================================
CREATE FUNCTION fn_GenerateOrderCode()
RETURNS NVARCHAR(20)
AS
BEGIN
    DECLARE @NextNum INT;
    SELECT @NextNum = ISNULL(MAX(CAST(SUBSTRING(OrderCode, 3, 10) AS INT)), 0) + 1 
    FROM Orders 
    WHERE OrderCode LIKE 'DH%';
    
    RETURN 'DH' + RIGHT('000' + CAST(@NextNum AS VARCHAR(3)), 3);
END;
GO

-- ===================================================
-- TEST QUERIES
-- ===================================================

-- Kiểm tra dữ liệu đã tạo
SELECT 'Tổng số xe trong hệ thống: ' + CAST(COUNT(*) AS NVARCHAR(10)) as Info FROM Cars;
SELECT 'Tổng số người dùng: ' + CAST(COUNT(*) AS NVARCHAR(10)) as Info FROM Users;
SELECT 'Tổng số đơn hàng: ' + CAST(COUNT(*) AS NVARCHAR(10)) as Info FROM Orders;
SELECT 'Tổng số hãng xe: ' + CAST(COUNT(*) AS NVARCHAR(10)) as Info FROM Brands;

-- Test view
SELECT * FROM vw_CarsWithDetails;
GO

-- Test stored procedure
EXEC sp_SearchCars @BrandID = 8; -- Tìm xe Toyota
GO

SELECT 'Database CarDealershipDB đã được tạo và cấu hình thành công!' as Message;
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';
