-- Drop tables in reverse dependency order (if needed)
DROP TABLE IF EXISTS "judgingRound";
DROP TABLE IF EXISTS "awardProcess";
DROP TABLE IF EXISTS "notification";
DROP TABLE IF EXISTS "announcement";
DROP TABLE IF EXISTS "feedbackResponse";
DROP TABLE IF EXISTS "feedbackFormQuestion";
DROP TABLE IF EXISTS "feedback";
DROP TABLE IF EXISTS "rsvpResponse";
DROP TABLE IF EXISTS "rsvpFormQuestion";
DROP TABLE IF EXISTS "rsvp";
DROP TABLE IF EXISTS "awardEvent";
DROP TABLE IF EXISTS "nominationAnswer";
DROP TABLE IF EXISTS "nominationQuestion";
DROP TABLE IF EXISTS "teamMember";
DROP TABLE IF EXISTS "nomination";
DROP TABLE IF EXISTS "nomineeSummary";
DROP TABLE IF EXISTS "awardCategory";
DROP TABLE IF EXISTS "mobileUserSettings";
DROP TABLE IF EXISTS "users";

-- Create Users Table
CREATE TABLE "users" (
    "Id" SERIAL PRIMARY KEY,
    "ExternalId" VARCHAR(255) NOT NULL UNIQUE,
    "WorkEmail" VARCHAR(255) NOT NULL UNIQUE,
    "Role" VARCHAR(50) NOT NULL,  -- e.g., 'employee', 'sponsor', 'admin'
    "CreatedAt" TIMESTAMP DEFAULT now(),
    "UpdatedAt" TIMESTAMP DEFAULT now()
);

-- Create Mobile User Settings Table
CREATE TABLE "mobileUserSettings" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL REFERENCES "users"("Id"),
    "PushNotifications" BOOLEAN DEFAULT true,
    "AiFunctionality" BOOLEAN DEFAULT true
);

-- Create Award Category Table
CREATE TABLE "awardCategory" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Type" VARCHAR(50) NOT NULL,  -- 'individual' or 'team'
    "SponsorId" INT REFERENCES "users"("Id"),
    "IntroductionVideo" VARCHAR(255),
    "IntroductionParagraph" TEXT,
    "ProfileStatus" VARCHAR(50) DEFAULT 'draft',  -- 'draft' or 'published'
    "CreatedAt" TIMESTAMP DEFAULT now(),
    "UpdatedAt" TIMESTAMP DEFAULT now()
);

-- Create Nominee Summary Table
CREATE TABLE "nomineeSummary" (
    "Id" SERIAL PRIMARY KEY,
    "NomineeId" INT NOT NULL REFERENCES "users"("Id"),
    "CategoryId" INT NOT NULL REFERENCES "awardCategory"("Id"),
    "TotalNominations" INT DEFAULT 0,
    "IsPinned" BOOLEAN DEFAULT false,
    "IsShortlisted" BOOLEAN DEFAULT false,
    "IsWinner" BOOLEAN DEFAULT false,
    "CreatedAt" TIMESTAMP DEFAULT now(),
    "UpdatedAt" TIMESTAMP DEFAULT now()
);

-- Create Nomination Table
CREATE TABLE "nomination" (
    "Id" SERIAL PRIMARY KEY,
    "CreatorId" INT NOT NULL REFERENCES "users"("Id"),
    "CategoryId" INT NOT NULL REFERENCES "awardCategory"("Id"),
    "NomineeId" INT REFERENCES "users"("Id"),
    "TeamName" VARCHAR(255),
    "AiSummary" TEXT,
    "VoteCount" INT DEFAULT 0,
    "Location" VARCHAR(50),
    "CreatedAt" TIMESTAMP DEFAULT now(),
    "UpdatedAt" TIMESTAMP DEFAULT now()
);

-- Create Team Member Table with surrogate key.
CREATE TABLE "teamMember" (
    "Id" SERIAL PRIMARY KEY,
    "NominationId" INT NOT NULL REFERENCES "nomination"("Id"),
    "UserId" INT NOT NULL REFERENCES "users"("Id"),
    CONSTRAINT "uq_teamMember" UNIQUE ("NominationId", "UserId")
);

-- Create Nomination Question Table
CREATE TABLE "nominationQuestion" (
    "Id" SERIAL PRIMARY KEY,
    "CategoryId" INT NOT NULL REFERENCES "awardCategory"("Id"),
    "QuestionText" TEXT NOT NULL
);

-- Create Nomination Answer Table with surrogate key.
CREATE TABLE "nominationAnswer" (
    "Id" SERIAL PRIMARY KEY,
    "NominationId" INT NOT NULL REFERENCES "nomination"("Id"),
    "QuestionId" INT NOT NULL REFERENCES "nominationQuestion"("Id"),
    "Answer" TEXT,
    CONSTRAINT "uq_nominationAnswer" UNIQUE ("NominationId", "QuestionId")
);

-- Create Award Event Table
CREATE TABLE "awardEvent" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Location" VARCHAR(255) NOT NULL,
    "EventDateTime" TIMESTAMP NOT NULL,
    "Description" TEXT,
    "Directions" VARCHAR(500) NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT now(),
    "UpdatedAt" TIMESTAMP DEFAULT now()
);

-- Create Rsvp Table
CREATE TABLE "rsvp" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL REFERENCES "users"("Id"),
    "EventId" INT NOT NULL REFERENCES "awardEvent"("Id"),
    "Status" VARCHAR(50) NOT NULL,  -- 'attending', 'not attending'
    "RsvpDate" TIMESTAMP DEFAULT now()
);

-- Create Rsvp Form Question Table
CREATE TABLE "rsvpFormQuestion" (
    "Id" SERIAL PRIMARY KEY,
    "EventId" INT NOT NULL REFERENCES "awardEvent"("Id"),
    "QuestionText" TEXT NOT NULL,
    "ResponseType" VARCHAR(50) NOT NULL,  -- 'text', 'yes/no', 'multiple choice'
    "Tooltip" TEXT,
    "QuestionOrder" INT
);

-- Create Rsvp Response Table with surrogate key.
CREATE TABLE "rsvpResponse" (
    "Id" SERIAL PRIMARY KEY,
    "RsvpId" INT NOT NULL REFERENCES "rsvp"("Id"),
    "QuestionId" INT NOT NULL REFERENCES "rsvpFormQuestion"("Id"),
    "Answer" TEXT,
    CONSTRAINT "uq_rsvpResponse" UNIQUE ("RsvpId", "QuestionId")
);

-- Create Feedback Table
CREATE TABLE "feedback" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL REFERENCES "users"("Id"),
    "EventId" INT NOT NULL REFERENCES "awardEvent"("Id"),
    "SubmittedAt" TIMESTAMP DEFAULT now()
);

-- Create Feedback Form Question Table
CREATE TABLE "feedbackFormQuestion" (
    "Id" SERIAL PRIMARY KEY,
    "EventId" INT NOT NULL REFERENCES "awardEvent"("Id"),
    "QuestionText" TEXT NOT NULL,
    "ResponseType" VARCHAR(50) NOT NULL,  -- 'text', 'yes/no', 'multiple choice'
    "Tooltip" TEXT,
    "QuestionOrder" INT
);

-- Create Feedback Response Table with surrogate key.
CREATE TABLE "feedbackResponse" (
    "Id" SERIAL PRIMARY KEY,
    "FeedbackId" INT NOT NULL REFERENCES "feedback"("Id"),
    "QuestionId" INT NOT NULL REFERENCES "feedbackFormQuestion"("Id"),
    "Answer" TEXT,
    CONSTRAINT "uq_feedbackResponse" UNIQUE ("FeedbackId", "QuestionId")
);

-- Create Announcement Table
CREATE TABLE "announcement" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "ImageUrl" VARCHAR(255),
    "IsPushNotification" BOOLEAN DEFAULT false,
    "ScheduledTime" TIMESTAMP,
    "Status" VARCHAR(50) NOT NULL,  -- 'draft' or 'published'
    "Type" VARCHAR(50) NOT NULL,    -- e.g., 'category', 'sponsor'
    "CreatedBy" INT REFERENCES "users"("Id"),
    "CreatedAt" TIMESTAMP DEFAULT now(),
    "UpdatedAt" TIMESTAMP DEFAULT now()
);

-- Create Notification Table
CREATE TABLE "notification" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL REFERENCES "users"("Id"),
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "Read" BOOLEAN DEFAULT false,
    "CreatedAt" TIMESTAMP DEFAULT now()
);

-- Create Award Process Table
CREATE TABLE "awardProcess" (
    "Id" SERIAL PRIMARY KEY,
    "AwardsName" VARCHAR(255) NOT NULL,
    "StartDate" TIMESTAMP NOT NULL,
    "EndDate" TIMESTAMP,
    "Status" VARCHAR(50) NOT NULL,  -- e.g., 'active', 'completed'
    "CreatedAt" TIMESTAMP DEFAULT now(),
    "UpdatedAt" TIMESTAMP DEFAULT now()
);

-- Create Judging Round Table
CREATE TABLE "judgingRound" (
    "Id" SERIAL PRIMARY KEY,
    "AwardProcessId" INT NOT NULL REFERENCES "awardProcess"("Id"),
    "RoundName" VARCHAR(255) NOT NULL,
    "StartDate" TIMESTAMP NOT NULL,
    "Deadline" TIMESTAMP NOT NULL,
    "CandidateCount" INT NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT now(),
    "UpdatedAt" TIMESTAMP DEFAULT now(),
    CONSTRAINT "chk_roundDates" CHECK ("Deadline" > "StartDate")
);