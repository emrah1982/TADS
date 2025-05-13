-- İlaçlama tablosu için örnek veri ekleme sorguları

-- Önce tabloyu kontrol edelim, yoksa oluşturalım
CREATE TABLE IF NOT EXISTS `pesticides` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userId` varchar(128) NOT NULL COMMENT 'Kullanıcı ID (GUID)',
  `fieldId` int(11) NOT NULL COMMENT 'Tarla ID',
  `date` datetime NOT NULL COMMENT 'Uygulama tarihi',
  `pesticideName` varchar(100) NOT NULL COMMENT 'İlaç adı',
  `dosage` decimal(10,2) NOT NULL COMMENT 'Doz miktarı',
  `pesticideType` varchar(50) DEFAULT NULL COMMENT 'İlaç türü',
  `targetPest` varchar(100) DEFAULT NULL COMMENT 'Hedef zararlı',
  `weatherCondition` varchar(50) DEFAULT NULL COMMENT 'Hava durumu',
  `notes` text DEFAULT NULL COMMENT 'Notlar',
  `createdAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `updatedAt` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_userId` (`userId`),
  KEY `idx_fieldId` (`fieldId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Örnek veriler ekleyelim
INSERT INTO `pesticides` 
(`userId`, `fieldId`, `date`, `pesticideName`, `dosage`, `pesticideType`, `targetPest`, `weatherCondition`, `notes`)
VALUES
-- Kullanıcı 1 için örnek veriler (GUID formatında kullanıcı ID'si)
('9fb8571e-380b-4bd3-bf86-f3108aea2679', 1, '2025-04-10 10:00:00', 'Roundup', 2.5, 'Herbisit', 'Yabani otlar', 'Açık ve güneşli', 'Sabah erken saatlerde uygulandı'),
('9fb8571e-380b-4bd3-bf86-f3108aea2679', 2, '2025-04-12 14:30:00', 'Karate', 1.8, 'İnsektisit', 'Yaprak biti', 'Parçalı bulutlu', 'Rüzgarsız havada uygulandı'),
('9fb8571e-380b-4bd3-bf86-f3108aea2679', 1, '2025-04-14 09:15:00', 'Ridomil', 3.0, 'Fungisit', 'Mildiyö', 'Bulutlu', 'Yağmur öncesi koruyucu olarak uygulandı'),

-- Kullanıcı 2 için örnek veriler (farklı bir GUID)
('7ac8642f-290a-4e5b-9f75-d2e87b45c123', 3, '2025-04-08 11:20:00', 'Decis', 1.2, 'İnsektisit', 'Kırmızı örümcek', 'Açık ve güneşli', 'Akşam serinliğinde uygulandı'),
('7ac8642f-290a-4e5b-9f75-d2e87b45c123', 4, '2025-04-11 16:45:00', 'Aliette', 2.0, 'Fungisit', 'Phytophthora', 'Yağmurlu', 'Yağmur sonrası uygulandı');

-- Kullanıcı ID'sini kontrol etmek için sorgu
-- SELECT * FROM pesticides WHERE userId = '9fb8571e-380b-4bd3-bf86-f3108aea2679';
