-- =============================================================
-- Новые таблицы для функционала выбора дисциплин
-- knowledge_test_db  |  MySQL InnoDB
-- Запускать ПОСЛЕ DB_FINAL_VERSION.sql
-- =============================================================

USE `knowledge_test_db`;

-- -------------------------------------------------------------
-- Таблица 1: group_period_status
-- Хранит связь: Группа – Семестр – Статус участия (Да/Нет)
-- -------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `group_period_status` (
  `id`          INT          NOT NULL AUTO_INCREMENT,
  `group_id`    INT          NOT NULL        COMMENT 'FK → study_groups.id',
  `semester_id` INT          NOT NULL        COMMENT 'FK → semesters.id',
  `status`      TINYINT(1)   NOT NULL DEFAULT 0
                                            COMMENT '1 = группа участвует, 0 = не участвует',
  `updated_by`  INT          NULL DEFAULT NULL COMMENT 'FK → users.id',
  `updated_at`  DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
                             ON UPDATE CURRENT_TIMESTAMP,

  PRIMARY KEY (`id`),
  UNIQUE  KEY `uq_gps_group_semester` (`group_id`, `semester_id`),
  INDEX        `idx_gps_group_id`     (`group_id`),
  INDEX        `idx_gps_semester_id`  (`semester_id`),

  CONSTRAINT `gps_fk_group`
    FOREIGN KEY (`group_id`)   REFERENCES `study_groups` (`id`) ON DELETE CASCADE,
  CONSTRAINT `gps_fk_semester`
    FOREIGN KEY (`semester_id`) REFERENCES `semesters`   (`id`) ON DELETE CASCADE,
  CONSTRAINT `gps_fk_user`
    FOREIGN KEY (`updated_by`) REFERENCES `users`        (`id`) ON DELETE SET NULL
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE  = utf8mb4_0900_ai_ci;


-- -------------------------------------------------------------
-- Таблица 2: group_selected_disciplines
-- Хранит две выбранные дисциплины + кафедру-владельца + флаг своя/чужая
-- Поля *_owner_department_id и *_is_own заполняются бэкендом автоматически
-- -------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `group_selected_disciplines` (
  `id`                              INT        NOT NULL AUTO_INCREMENT,
  `group_id`                        INT        NOT NULL COMMENT 'FK → study_groups.id',
  `semester_id`                     INT        NOT NULL COMMENT 'FK → semesters.id',

  `discipline1_id`                  INT        NULL DEFAULT NULL COMMENT 'FK → disciplines.id',
  `discipline1_owner_department_id` INT        NULL DEFAULT NULL
                                               COMMENT 'FK → departments.id  (ставит бэкенд)',
  `discipline1_is_own`              TINYINT(1) NULL DEFAULT NULL
                                               COMMENT '1 = своя кафедра, 0 = чужая',

  `discipline2_id`                  INT        NULL DEFAULT NULL COMMENT 'FK → disciplines.id',
  `discipline2_owner_department_id` INT        NULL DEFAULT NULL
                                               COMMENT 'FK → departments.id  (ставит бэкенд)',
  `discipline2_is_own`              TINYINT(1) NULL DEFAULT NULL
                                               COMMENT '1 = своя кафедра, 0 = чужая',

  `selected_by`                     INT        NULL DEFAULT NULL COMMENT 'FK → users.id',
  `updated_at`                      DATETIME   NOT NULL DEFAULT CURRENT_TIMESTAMP
                                               ON UPDATE CURRENT_TIMESTAMP,

  PRIMARY KEY (`id`),
  UNIQUE  KEY `uq_gsd_group_semester` (`group_id`, `semester_id`),
  INDEX        `idx_gsd_group_id`     (`group_id`),
  INDEX        `idx_gsd_semester_id`  (`semester_id`),

  CONSTRAINT `gsd_fk_group`
    FOREIGN KEY (`group_id`)                        REFERENCES `study_groups` (`id`) ON DELETE CASCADE,
  CONSTRAINT `gsd_fk_semester`
    FOREIGN KEY (`semester_id`)                     REFERENCES `semesters`   (`id`) ON DELETE CASCADE,
  CONSTRAINT `gsd_fk_disc1`
    FOREIGN KEY (`discipline1_id`)                  REFERENCES `disciplines` (`id`) ON DELETE SET NULL,
  CONSTRAINT `gsd_fk_disc2`
    FOREIGN KEY (`discipline2_id`)                  REFERENCES `disciplines` (`id`) ON DELETE SET NULL,
  CONSTRAINT `gsd_fk_dept1`
    FOREIGN KEY (`discipline1_owner_department_id`) REFERENCES `departments` (`id`) ON DELETE SET NULL,
  CONSTRAINT `gsd_fk_dept2`
    FOREIGN KEY (`discipline2_owner_department_id`) REFERENCES `departments` (`id`) ON DELETE SET NULL,
  CONSTRAINT `gsd_fk_user`
    FOREIGN KEY (`selected_by`)                     REFERENCES `users`       (`id`) ON DELETE SET NULL
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE  = utf8mb4_0900_ai_ci;
