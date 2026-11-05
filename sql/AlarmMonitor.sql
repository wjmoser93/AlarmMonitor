-- --------------------------------------------------------
-- Host:                         172.19.1.31
-- Server version:               8.4.11-0ubuntu0.26.04.1 - (Ubuntu)
-- Server OS:                    Linux
-- HeidiSQL Version:             12.21.0.7344
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for AlarmMonitor
CREATE DATABASE IF NOT EXISTS `AlarmMonitor` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `AlarmMonitor`;

-- Dumping structure for table AlarmMonitor.alarm_dest
CREATE TABLE IF NOT EXISTS `alarm_dest` (
  `alarm_dest_id` int NOT NULL AUTO_INCREMENT,
  `priority` tinyint NOT NULL,
  `type` tinyint NOT NULL,
  `dst` varchar(50) COLLATE utf8mb4_general_ci NOT NULL,
  `active_open` tinyint(1) NOT NULL,
  `active_close` tinyint(1) NOT NULL,
  `active_sunday` tinyint(1) NOT NULL,
  `auto_ack` tinyint(1) NOT NULL,
  `dst_name` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`alarm_dest_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table AlarmMonitor.alarm_history
CREATE TABLE IF NOT EXISTS `alarm_history` (
  `action` varchar(8) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT 'insert',
  `revision` int NOT NULL AUTO_INCREMENT,
  `dt_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `alarm_id` int NOT NULL,
  `store` text COLLATE utf8mb4_general_ci NOT NULL,
  `rack_name` text COLLATE utf8mb4_general_ci NOT NULL,
  `alarm_type` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `alarm_unit` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `alarm_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `alarm_value` text COLLATE utf8mb4_general_ci NOT NULL,
  `alarm_cleared` tinyint(1) NOT NULL DEFAULT '0',
  `alarm_ack` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`alarm_id`,`revision`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table AlarmMonitor.alarm_inputs
CREATE TABLE IF NOT EXISTS `alarm_inputs` (
  `alarm_input_id` int NOT NULL AUTO_INCREMENT,
  `input_name` varchar(50) COLLATE utf8mb4_general_ci NOT NULL,
  `input_store` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ignore_always` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`alarm_input_id`)
) ENGINE=InnoDB AUTO_INCREMENT=319 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table AlarmMonitor.alarm_last_test
CREATE TABLE IF NOT EXISTS `alarm_last_test` (
  `store` varchar(64) COLLATE utf8mb4_general_ci NOT NULL,
  `last_alarm_id` int NOT NULL,
  `last_alarm_time` datetime NOT NULL,
  `alarm_unit` varchar(64) COLLATE utf8mb4_general_ci NOT NULL,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`store`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table AlarmMonitor.alarm_notify
CREATE TABLE IF NOT EXISTS `alarm_notify` (
  `alarm_notify_id` int NOT NULL AUTO_INCREMENT,
  `alarm_id` int NOT NULL,
  `dst_id` varchar(20) COLLATE utf8mb4_general_ci NOT NULL,
  `notify_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ackd` tinyint(1) NOT NULL DEFAULT '0',
  `ackd_time` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`alarm_notify_id`),
  KEY `alarm_id` (`alarm_id`),
  CONSTRAINT `alarm_notify_ibfk_1` FOREIGN KEY (`alarm_id`) REFERENCES `alarms` (`alarm_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table AlarmMonitor.alarms
CREATE TABLE IF NOT EXISTS `alarms` (
  `alarm_id` int NOT NULL AUTO_INCREMENT,
  `store` text COLLATE utf8mb4_general_ci NOT NULL,
  `rack_name` text COLLATE utf8mb4_general_ci NOT NULL,
  `alarm_type` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `alarm_unit` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `alarm_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `alarm_value` text COLLATE utf8mb4_general_ci NOT NULL,
  `alarm_cleared` tinyint(1) NOT NULL DEFAULT '0',
  `alarm_ack` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`alarm_id`)
) ENGINE=InnoDB AUTO_INCREMENT=14919 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for view AlarmMonitor.data_history_changes
-- Creating temporary table to overcome VIEW dependency errors
CREATE TABLE `data_history_changes` (
	`dt_datetime` DATETIME NOT NULL,
	`action` VARCHAR(1) NOT NULL COLLATE 'utf8mb4_general_ci',
	`row id` INT NOT NULL,
	`alarm_cleared` VARCHAR(1) NOT NULL COLLATE 'utf8mb3_general_ci'
);

-- Dumping structure for trigger AlarmMonitor.data__ai
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='IGNORE_SPACE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `data__ai` AFTER INSERT ON `alarms` FOR EACH ROW INSERT INTO AlarmMonitor.alarm_history SELECT 'insert', NULL, NOW(), d.* 
    FROM AlarmMonitor.alarms AS d WHERE d.alarm_id = NEW.alarm_id//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Dumping structure for trigger AlarmMonitor.data__au
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='IGNORE_SPACE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `data__au` AFTER UPDATE ON `alarms` FOR EACH ROW INSERT INTO AlarmMonitor.alarm_history SELECT 'update', NULL, NOW(), d.*
    FROM AlarmMonitor.alarms AS d WHERE d.alarm_id = NEW.alarm_id//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Dumping structure for trigger AlarmMonitor.data__bd
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='IGNORE_SPACE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `data__bd` BEFORE DELETE ON `alarms` FOR EACH ROW INSERT INTO AlarmMonitor.alarm_history SELECT 'delete', NULL, NOW(), d.* 
    FROM AlarmMonitor.alarms AS d WHERE d.alarm_id = OLD.alarm_id//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Dumping structure for trigger AlarmMonitor.trg_alarms_after_insert
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='IGNORE_SPACE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `trg_alarms_after_insert` AFTER INSERT ON `alarms` FOR EACH ROW BEGIN
    IF NEW.alarm_type = 'Test Alarm' THEN
        INSERT INTO alarm_last_test (store, last_alarm_id, last_alarm_time, alarm_unit)
        VALUES (NEW.store, NEW.alarm_id, NEW.alarm_time, NEW.alarm_unit)
        ON DUPLICATE KEY UPDATE
            last_alarm_id = VALUES(last_alarm_id),
            last_alarm_time = VALUES(last_alarm_time),
            alarm_unit = VALUES(alarm_unit);
    END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `data_history_changes`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `data_history_changes` AS select `t2`.`dt_datetime` AS `dt_datetime`,`t2`.`action` AS `action`,`t1`.`alarm_id` AS `row id`,if((`t1`.`alarm_cleared` = `t2`.`alarm_cleared`),`t1`.`alarm_cleared`,concat(`t1`.`alarm_cleared`,' to ',`t2`.`alarm_cleared`)) AS `alarm_cleared` from (`alarm_history` `t1` join `alarm_history` `t2` on((`t1`.`alarm_id` = `t2`.`alarm_id`))) where (((`t1`.`revision` = 1) and (`t2`.`revision` = 1)) or (`t2`.`revision` = (`t1`.`revision` + 1))) order by `t1`.`alarm_id`,`t2`.`revision`
;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
