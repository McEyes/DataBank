/**
 * Options for Right and Left containers in Layout
 */
export declare enum LayoutPanelPosition {
    Default = "Default",
    Fixed = "Fixed",
    Hidden = "Hidden",
    Hover = "Hover"
}
export interface LayoutConfig {
    /**
     * Position of right container
     */
    rightContainerPosition?: LayoutPanelPosition;
    /**
     * shift right container to left, to display right content part when collapsed
     */
    shiftRightContentWithHoverMode?: boolean;
    /**
     * Position of left container
     */
    leftContainerPosition?: LayoutPanelPosition;
    /**
     * shift left container to right, to display left content part when collapsed
     */
    shiftLeftContentWithHoverMode?: boolean;
    /**
     * Switch left and right containers, in case they are fixed: boolean
     */
    toggleLeftAndRightPosition?: boolean;
    /**
     * Make hidden container full screen on devices less than 992px: boolean
     */
    fullWidthOnSmallScreen?: boolean;
}
