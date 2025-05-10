import React from 'react';

const AestheticImage: React.FC = () => {
    return (
        <div className="w-full h-70 bg-cover bg-center rounded border border-gray-300 dark:border-gray-600" style={{ backgroundImage: 'url(/src/assets/feedback-image.png)' }}>
            {/* Optional overlay or content */}
        </div>
    );
};

export default AestheticImage;
