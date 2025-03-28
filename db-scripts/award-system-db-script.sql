-- Drop tables in reverse dependency order (if needed)
DROP TABLE IF EXISTS judging_round;
DROP TABLE IF EXISTS award_process;
DROP TABLE IF EXISTS notification;
DROP TABLE IF EXISTS announcement;
DROP TABLE IF EXISTS feedback_response;
DROP TABLE IF EXISTS feedback_form_question;
DROP TABLE IF EXISTS feedback;
DROP TABLE IF EXISTS rsvp_response;
DROP TABLE IF EXISTS rsvp_form_question;
DROP TABLE IF EXISTS rsvp;
DROP TABLE IF EXISTS award_event;
DROP TABLE IF EXISTS nomination_answer;
DROP TABLE IF EXISTS nomination_question;
DROP TABLE IF EXISTS team_member;
DROP TABLE IF EXISTS nomination;
DROP TABLE IF EXISTS nominee_summary;
DROP TABLE IF EXISTS award_category;
DROP TABLE IF EXISTS mobile_user_settings;
DROP TABLE IF EXISTS users;

-- Create Users Table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    external_id VARCHAR(255) NOT NULL UNIQUE,
    work_email VARCHAR(255) NOT NULL UNIQUE,
    role VARCHAR(50) NOT NULL,  -- e.g., 'employee', 'sponsor', 'admin'
    created_at TIMESTAMP DEFAULT now(),
    updated_at TIMESTAMP DEFAULT now()
);

-- Create Mobile User Settings Table
CREATE TABLE mobile_user_settings (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    push_notifications BOOLEAN DEFAULT true,
    ai_functionality BOOLEAN DEFAULT true
);

-- Create Award Category Table
CREATE TABLE award_category (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    type VARCHAR(50) NOT NULL,  -- 'individual' or 'team'
    sponsor_id INT REFERENCES users(id),
    introduction_video VARCHAR(255),
    introduction_paragraph TEXT,
    profile_status VARCHAR(50) DEFAULT 'draft',  -- 'draft' or 'published'
    created_at TIMESTAMP DEFAULT now(),
    updated_at TIMESTAMP DEFAULT now()
);

-- Create Nominee Summary Table
CREATE TABLE nominee_summary (
    id SERIAL PRIMARY KEY,
    nominee_id INT NOT NULL REFERENCES users(id),
    category_id INT NOT NULL REFERENCES award_category(id),
    total_nominations INT DEFAULT 0,
    is_pinned BOOLEAN DEFAULT false,
    is_shortlisted BOOLEAN DEFAULT false,
    is_winner BOOLEAN DEFAULT false,
    updated_at TIMESTAMP DEFAULT now()
);

-- Create Nomination Table
CREATE TABLE nomination (
    id SERIAL PRIMARY KEY,
    creator_id INT NOT NULL REFERENCES users(id),
    category_id INT NOT NULL REFERENCES award_category(id),
    nominee_id INT REFERENCES users(id),
    team_name VARCHAR(255),
    ai_summary TEXT,
    vote_count INT DEFAULT 0,
    location VARCHAR(50),
    created_at TIMESTAMP DEFAULT now(),
    updated_at TIMESTAMP DEFAULT now()
);

-- Create Team Member Table with surrogate key.
CREATE TABLE team_member (
    id SERIAL PRIMARY KEY,
    nomination_id INT NOT NULL REFERENCES nomination(id),
    user_id INT NOT NULL REFERENCES users(id),
    CONSTRAINT uq_team_member UNIQUE (nomination_id, user_id)
);

-- Create Nomination Question Table
CREATE TABLE nomination_question (
    id SERIAL PRIMARY KEY,
    category_id INT NOT NULL REFERENCES award_category(id),
    question_text TEXT NOT NULL
);

-- Create Nomination Answer Table with surrogate key.
CREATE TABLE nomination_answer (
    id SERIAL PRIMARY KEY,
    nomination_id INT NOT NULL REFERENCES nomination(id),
    question_id INT NOT NULL REFERENCES nomination_question(id),
    answer TEXT,
    CONSTRAINT uq_nomination_answer UNIQUE (nomination_id, question_id)
);

-- Create Award Event Table
CREATE TABLE award_event (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    location VARCHAR(255) NOT NULL,
    event_date_time TIMESTAMP NOT NULL,
    description TEXT,
    directions VARCHAR(500) NOT NULL,
    created_at TIMESTAMP DEFAULT now(),
    updated_at TIMESTAMP DEFAULT now()
);

-- Create RSVP Table
CREATE TABLE rsvp (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    event_id INT NOT NULL REFERENCES award_event(id),
    status VARCHAR(50) NOT NULL,  -- 'attending', 'not attending'
    rsvp_date TIMESTAMP DEFAULT now()
);

-- Create RSVP Form Question Table
CREATE TABLE rsvp_form_question (
    id SERIAL PRIMARY KEY,
    event_id INT NOT NULL REFERENCES award_event(id),
    question_text TEXT NOT NULL,
    response_type VARCHAR(50) NOT NULL,  -- 'text', 'yes/no', 'multiple choice'
    tooltip TEXT,
    question_order INT
);

-- Create RSVP Response Table with surrogate key.
CREATE TABLE rsvp_response (
    id SERIAL PRIMARY KEY,
    rsvp_id INT NOT NULL REFERENCES rsvp(id),
    question_id INT NOT NULL REFERENCES rsvp_form_question(id),
    answer TEXT,
    CONSTRAINT uq_rsvp_response UNIQUE (rsvp_id, question_id)
);

-- Create Feedback Table
CREATE TABLE feedback (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    event_id INT NOT NULL REFERENCES award_event(id),
    submitted_at TIMESTAMP DEFAULT now()
);

-- Create Feedback Form Question Table
CREATE TABLE feedback_form_question (
    id SERIAL PRIMARY KEY,
    event_id INT NOT NULL REFERENCES award_event(id),
    question_text TEXT NOT NULL,
    response_type VARCHAR(50) NOT NULL,  -- 'text', 'yes/no', 'multiple choice'
    tooltip TEXT,
    question_order INT
);

-- Create Feedback Response Table with surrogate key.
CREATE TABLE feedback_response (
    id SERIAL PRIMARY KEY,
    feedback_id INT NOT NULL REFERENCES feedback(id),
    question_id INT NOT NULL REFERENCES feedback_form_question(id),
    answer TEXT,
    CONSTRAINT uq_feedback_response UNIQUE (feedback_id, question_id)
);

-- Create Announcement Table
CREATE TABLE announcement (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    image_url VARCHAR(255),
    is_push_notification BOOLEAN DEFAULT false,
    scheduled_time TIMESTAMP,
    status VARCHAR(50) NOT NULL,  -- 'draft' or 'published'
    type VARCHAR(50) NOT NULL,    -- e.g., 'category', 'sponsor'
    created_by INT REFERENCES users(id),
    created_at TIMESTAMP DEFAULT now(),
    updated_at TIMESTAMP DEFAULT now()
);

-- Create Notification Table
CREATE TABLE notification (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    read BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT now()
);

-- Create Award Process Table
CREATE TABLE award_process (
   id SERIAL PRIMARY KEY,
   start_date TIMESTAMP NOT NULL,
   end_date TIMESTAMP,
   status VARCHAR(50) NOT NULL,  -- e.g., 'active', 'completed'
   created_at TIMESTAMP DEFAULT now()
);

-- Create Judging Round Table
CREATE TABLE judging_round (
    id SERIAL PRIMARY KEY,
    award_process_id INT NOT NULL REFERENCES award_process(id),
    round_name VARCHAR(255) NOT NULL,
    start_date TIMESTAMP NOT NULL,
    deadline TIMESTAMP NOT NULL,
    candidate_count INT NOT NULL,
    created_at TIMESTAMP DEFAULT now(),
    updated_at TIMESTAMP DEFAULT now(),
    CONSTRAINT chk_round_dates CHECK (deadline > start_date)
);
