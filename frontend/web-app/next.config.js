/** @type {import('next').NextConfig} */

module.exports = {
    logging: {
      fetches: {
        fullUrl: true,
      },
    },
    images: {
      remotePatterns: [
        {
          protocol: 'https',
          hostname: 'cdn.pixabay.com',
          port: '',
          pathname: '/**',
        },
      ],
    }
  }