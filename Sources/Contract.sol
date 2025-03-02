// SPDX-License-Identifier: MIT
pragma solidity ^0.8.7;

import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/release-v4.0/contracts/token/ERC721/ERC721.sol";
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/release-v4.0/contracts/access/Ownable.sol";
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/release-v4.0/contracts/utils/Counters.sol";

contract AIMetaVerseGame is ERC721, Ownable {

    string constant _valuer = "v1.0001";

    using Counters for Counters.Counter;
    Counters.Counter private _tokenIds;
    
    mapping(uint256 => string) private _tokenURIs;
    mapping(address => uint256[]) private _ownedTokens;

    event Minted(address indexed owner, uint256 indexed tokenId, string tokenURI);
    event Transferred(address indexed from, address indexed to, uint256 indexed tokenId);
    
    constructor() ERC721("AIMetaSocial", "AIMS") {}

    // Function to mint a new NFT
    function mintNFT(string memory tokenURI) external returns (uint256) {
        _tokenIds.increment(); // Increment token ID counter
        uint256 newItemId = _tokenIds.current(); // Get new token ID
        _mint(msg.sender, newItemId); // Mint NFT to sender
        _setTokenURI(newItemId, tokenURI); // Set token metadata URI
        _ownedTokens[msg.sender].push(newItemId); // Store NFT ownership
        emit Minted(msg.sender, newItemId, tokenURI); // Emit event for minting
        return newItemId;
    }
    
    // Internal function to set token URI
    function _setTokenURI(uint256 tokenId, string memory tokenURI) internal {
        _tokenURIs[tokenId] = tokenURI;
    }
    
    // Function to retrieve token URI (image or metadata)
    function tokenURI(uint256 tokenId) public view override returns (string memory) {
        require(_exists(tokenId), "ERC721Metadata: URI query for nonexistent token");
        return _tokenURIs[tokenId];
    }
    
    // Function to retrieve all NFTs owned by a user
    function getOwnedNFTs(address owner) external view returns (uint256[] memory) {
        return _ownedTokens[owner];
    }

    // Function to retrieve all owned NFT URIs
    function getOwnedNFTURIs(address owner) external view returns (string[] memory) {
        uint256[] memory tokenIds = _ownedTokens[owner];
        string[] memory uris = new string[](tokenIds.length);
        for (uint256 i = 0; i < tokenIds.length; i++) {
            uris[i] = _tokenURIs[tokenIds[i]];
        }
        return uris;
    }

    // Override transfer function to track ownership changes
    function _transfer(address from, address to, uint256 tokenId) internal override {
        super._transfer(from, to, tokenId);
        _removeTokenFromOwner(from, tokenId);
        _ownedTokens[to].push(tokenId);
        emit Transferred(from, to, tokenId);
    }

    // Internal function to remove a token from the previous owner's list
    function _removeTokenFromOwner(address owner, uint256 tokenId) internal {
        uint256[] storage tokens = _ownedTokens[owner];
        for (uint256 i = 0; i < tokens.length; i++) {
            if (tokens[i] == tokenId) {
                tokens[i] = tokens[tokens.length - 1]; // Move last token to current index
                tokens.pop(); // Remove last token
                break;
            }
        }
    }

    function purchaseCoins(uint256 _itemId) payable public {
        // Implementation for buying coins
    }

    function getCurrentTime() public view returns (uint256) {
        return block.timestamp;
    }

    function withdraw(address _recipient) public payable onlyOwner {
        payable(_recipient).transfer(address(this).balance);
    }
}
