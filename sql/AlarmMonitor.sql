SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

CREATE DATABASE IF NOT EXISTS `AlarmMonitor` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `AlarmMonitor`;

CREATE TABLE `alarms` (
  `alarm_id` int NOT NULL,
  `store` text COLLATE utf8mb4_general_ci NOT NULL,
  `rack_name` text COLLATE utf8mb4_general_ci NOT NULL,
  `alarm_type` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `alarm_unit` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `alarm_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `alarm_value` text COLLATE utf8mb4_general_ci NOT NULL,
  `alarm_cleared` tinyint(1) NOT NULL DEFAULT '0',
  `alarm_ack` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
DELIMITER $$
CREATE TRIGGER `data__ai` AFTER INSERT ON `alarms` FOR EACH ROW INSERT INTO AlarmMonitor.alarm_history SELECT 'insert', NULL, NOW(), d.* 
    FROM AlarmMonitor.alarms AS d WHERE d.alarm_id = NEW.alarm_id
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `data__au` AFTER UPDATE ON `alarms` FOR EACH ROW INSERT INTO AlarmMonitor.alarm_history SELECT 'update', NULL, NOW(), d.*
    FROM AlarmMonitor.alarms AS d WHERE d.alarm_id = NEW.alarm_id
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `data__bd` BEFORE DELETE ON `alarms` FOR EACH ROW INSERT INTO AlarmMonitor.alarm_history SELECT 'delete', NULL, NOW(), d.* 
    FROM AlarmMonitor.alarms AS d WHERE d.alarm_id = OLD.alarm_id
$$
DELIMITER ;

CREATE TABLE `alarm_dest` (
  `alarm_dest_id` int NOT NULL,
  `priority` tinyint NOT NULL,
  `type` tinyint NOT NULL,
  `dst` varchar(50) COLLATE utf8mb4_general_ci NOT NULL,
  `active_open` tinyint(1) NOT NULL,
  `active_close` tinyint(1) NOT NULL,
  `active_sunday` tinyint(1) NOT NULL,
  `auto_ack` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `alarm_history` (
  `action` varchar(8) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT 'insert',
  `revision` int NOT NULL,
  `dt_datetime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `alarm_id` int NOT NULL,
  `store` text COLLATE utf8mb4_general_ci NOT NULL,
  `rack_name` text COLLATE utf8mb4_general_ci NOT NULL,
  `alarm_type` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `alarm_unit` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `alarm_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `alarm_value` text COLLATE utf8mb4_general_ci NOT NULL,
  `alarm_cleared` tinyint(1) NOT NULL DEFAULT '0',
  `alarm_ack` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `alarm_inputs` (
  `alarm_input_id` int NOT NULL,
  `input_name` varchar(50) COLLATE utf8mb4_general_ci NOT NULL,
  `input_store` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ignore_always` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `alarm_notify` (
  `alarm_notify_id` int NOT NULL,
  `alarm_id` int NOT NULL,
  `dst_id` int NOT NULL,
  `notify_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ackd` tinyint(1) NOT NULL DEFAULT '0',
  `ackd_time` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
CREATE TABLE `data_history_changes` (
`action` varchar(8)
,`alarm_cleared` varchar(12)
,`dt_datetime` datetime
,`row id` int
);
DROP TABLE IF EXISTS `data_history_changes`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `data_history_changes`  AS  select `t2`.`dt_datetime` AS `dt_datetime`,`t2`.`action` AS `action`,`t1`.`alarm_id` AS `row id`,if((`t1`.`alarm_cleared` = `t2`.`alarm_cleared`),`t1`.`alarm_cleared`,concat(`t1`.`alarm_cleared`,' to ',`t2`.`alarm_cleared`)) AS `alarm_cleared` from (`alarm_history` `t1` join `alarm_history` `t2` on((`t1`.`alarm_id` = `t2`.`alarm_id`))) where (((`t1`.`revision` = 1) and (`t2`.`revision` = 1)) or (`t2`.`revision` = (`t1`.`revision` + 1))) order by `t1`.`alarm_id`,`t2`.`revision` ;


ALTER TABLE `alarms`
  ADD PRIMARY KEY (`alarm_id`);

ALTER TABLE `alarm_dest`
  ADD PRIMARY KEY (`alarm_dest_id`);

ALTER TABLE `alarm_history`
  ADD PRIMARY KEY (`alarm_id`,`revision`);

ALTER TABLE `alarm_inputs`
  ADD PRIMARY KEY (`alarm_input_id`);

ALTER TABLE `alarm_notify`
  ADD PRIMARY KEY (`alarm_notify_id`),
  ADD KEY `alarm_id` (`alarm_id`),
  ADD KEY `dst_id` (`dst_id`);


ALTER TABLE `alarms`
  MODIFY `alarm_id` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `alarm_dest`
  MODIFY `alarm_dest_id` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `alarm_history`
  MODIFY `revision` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `alarm_inputs`
  MODIFY `alarm_input_id` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `alarm_notify`
  MODIFY `alarm_notify_id` int NOT NULL AUTO_INCREMENT;


ALTER TABLE `alarm_notify`
  ADD CONSTRAINT `alarm_notify_ibfk_1` FOREIGN KEY (`alarm_id`) REFERENCES `alarms` (`alarm_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `alarm_notify_ibfk_2` FOREIGN KEY (`dst_id`) REFERENCES `alarm_dest` (`alarm_dest_id`) ON DELETE CASCADE ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
