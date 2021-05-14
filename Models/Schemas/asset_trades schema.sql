DROP TABLE IF EXISTS `transactions`.`asset_trades`;
CREATE TABLE `transactions`.`asset_trades` (
  `sequence` INT(11) NOT NULL AUTO_INCREMENT,
  `trade_time` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `trade_id` VARCHAR(36) NOT NULL,
  `game_id` VARCHAR(36) NOT NULL,
  `buyer` VARCHAR(36) NOT NULL,
  `seller` VARCHAR(36) NOT NULL,
  `asset` VARCHAR(36) NOT NULL,
  `price` DECIMAL(11,3) UNSIGNED NOT NULL,
  UNIQUE INDEX `sequence` (`sequence` ASC),
  PRIMARY KEY (`sequence`),
  INDEX `game_id` (`game_id` ASC),
  INDEX `buyer` (`buyer`),
  INDEX `seller` (`seller`),
  INDEX `asset` (`asset`));