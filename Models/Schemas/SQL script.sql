-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema Transactions
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema Transactions
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `Transactions` ;
USE `Transactions` ;

-- -----------------------------------------------------
-- Table `Transactions`.`asset_trades`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Transactions`.`asset_trades` (
  `sequence` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `trade_id` VARCHAR(36) NOT NULL,
  `game_id` VARCHAR(36) NOT NULL,
  `buyer` VARCHAR(36) NOT NULL,
  `seller` VARCHAR(36) NOT NULL,
  `asset` VARCHAR(36) NOT NULL,
  `price` DECIMAL(3) UNSIGNED NOT NULL,
  `trade_time` DATETIME NOT NULL,
  `company_asset_value` DECIMAL(3) NOT NULL,
  UNIQUE INDEX `trade_id_UNIQUE` (`trade_id`),
  UNIQUE INDEX `sequence_UNIQUE` (`sequence`),
  PRIMARY KEY (`sequence`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
